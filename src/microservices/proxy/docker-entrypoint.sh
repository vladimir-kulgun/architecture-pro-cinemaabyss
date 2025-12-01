#!/bin/sh
set -e

PORT=${PORT:-8000}
MOVIES_WEIGHT_NEW=${MOVIES_MIGRATION_PERCENT:-50}
MOVIES_WEIGHT_OLD=$((100 - MOVIES_WEIGHT_NEW))

MONOLITH_HOST=${MONOLITH_URL#http://}
MOVIES_SERVICE_HOST=${MOVIES_SERVICE_URL#http://}

wait_for_host() {
  host=$1
  port=$2
  timeout=${3:-60}
  start_time=$(date +%s)

  while ! nc -z "$host" "$port"; do
    echo "Waiting for $host:$port..."
    sleep 1

    if (( $(date +%s) - start_time > timeout )); then
      echo "Timeout waiting for $host:$port"
      return 1
    fi
  done

  echo "$host:$port is available!"
}


wait_for_host ${MONOLITH_HOST%:*} ${MONOLITH_HOST#*:}
wait_for_host ${MOVIES_SERVICE_HOST%:*} ${MOVIES_SERVICE_HOST#*:}

# ---------- Build nginx.conf ----------
NGINX_CONF="/etc/nginx/nginx.conf"

cat > $NGINX_CONF <<EOF
events {}

http {
    upstream movies_backend {
EOF

if [ "$GRADUAL_MIGRATION" = "true" ]; then

  #
  # CASE A: MOVIES_WEIGHT_NEW = 0 → FAILOVER SERVER ONLY
  #
  if [ "$MOVIES_WEIGHT_NEW" -eq 0 ]; then
    echo "        server ${MONOLITH_HOST} weight=100;" >> $NGINX_CONF
    echo "        server ${MOVIES_SERVICE_HOST} backup;" >> $NGINX_CONF

  #
  # CASE B: MOVIES_WEIGHT_NEW = 100 → full migration to new service
  #
  elif [ "$MOVIES_WEIGHT_NEW" -eq 100 ]; then
    echo "        server ${MONOLITH_HOST} backup;" >> $NGINX_CONF
    echo "        server ${MOVIES_SERVICE_HOST} weight=100;" >> $NGINX_CONF

  #
  # CASE C: Between 1 and 99 — weighted balancing
  #
  else
    echo "        server ${MONOLITH_HOST} weight=${MOVIES_WEIGHT_OLD};" >> $NGINX_CONF
    echo "        server ${MOVIES_SERVICE_HOST} weight=${MOVIES_WEIGHT_NEW};" >> $NGINX_CONF
  fi

else
  # No gradual migration → send all traffic to monolith
  echo "        server ${MONOLITH_HOST};" >> $NGINX_CONF
fi

cat >> $NGINX_CONF <<EOF
    }

    server {
        listen ${PORT};

        location /health {
          access_log off;
          return 200 "OK";
        }

        location /api/movies {
            proxy_pass http://movies_backend;
            proxy_set_header Host \$host;
            proxy_set_header X-Real-IP \$remote_addr;
            proxy_set_header X-Forwarded-For \$proxy_add_x_forwarded_for;
            proxy_set_header X-Forwarded-Proto \$scheme;
        }

        location /api/users {
            proxy_pass http://${MONOLITH_HOST};
            proxy_set_header Host \$host;
            proxy_set_header X-Real-IP \$remote_addr;
            proxy_set_header X-Forwarded-For \$proxy_add_x_forwarded_for;
            proxy_set_header X-Forwarded-Proto \$scheme;
        }
    }
}
EOF

nginx -g 'daemon off;'