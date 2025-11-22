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
  while ! nc -z $host $port; do
    echo "Waiting for $host:$port..."
    sleep 1
  done
}

wait_for_host ${MONOLITH_HOST%:*} ${MONOLITH_HOST#*:}
wait_for_host ${MOVIES_SERVICE_HOST%:*} ${MOVIES_SERVICE_HOST#*:}

NGINX_CONF="/etc/nginx/nginx.conf"

cat > $NGINX_CONF <<EOF
events {}

http {
    upstream movies_backend {
EOF

if [ "$GRADUAL_MIGRATION" = "true" ]; then
  echo "        server ${MONOLITH_HOST} weight=${MOVIES_WEIGHT_OLD};" >> $NGINX_CONF
  echo "        server ${MOVIES_SERVICE_HOST} weight=${MOVIES_WEIGHT_NEW};" >> $NGINX_CONF
else
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
