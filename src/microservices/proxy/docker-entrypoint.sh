#!/bin/sh
set -e

PORT=${PORT:-8000}
MOVIES_WEIGHT_NEW=${MOVIES_MIGRATION_PERCENT:-50}
MOVIES_WEIGHT_OLD=$((100 - MOVIES_WEIGHT_NEW))
EVENTS_WEIGHT_NEW=${EVENTS_MIGRATION_PERCENT:-50}
EVENTS_WEIGHT_OLD=$((100 - EVENTS_WEIGHT_NEW))

# Убираем http:// для upstream
MONOLITH_HOST=${MONOLITH_URL#http://}
MOVIES_SERVICE_HOST=${MOVIES_SERVICE_URL#http://}
EVENTS_SERVICE_HOST=${EVENTS_SERVICE_URL#http://}

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

    upstream events_backend {
EOF

if [ "$GRADUAL_MIGRATION" = "true" ]; then
  echo "        server ${MONOLITH_HOST} weight=${EVENTS_WEIGHT_OLD};" >> $NGINX_CONF
  echo "        server ${EVENTS_SERVICE_HOST} weight=${EVENTS_WEIGHT_NEW};" >> $NGINX_CONF
else
  echo "        server ${MONOLITH_HOST};" >> $NGINX_CONF
fi

cat >> $NGINX_CONF <<EOF
    }

    server {
        listen ${PORT};

        location /api/movies {
            proxy_pass http://movies_backend;
            proxy_set_header Host \$host;
            proxy_set_header X-Real-IP \$remote_addr;
            proxy_set_header X-Forwarded-For \$proxy_add_x_forwarded_for;
            proxy_set_header X-Forwarded-Proto \$scheme;
        }

        location /api/events {
            proxy_pass http://events_backend;
            proxy_set_header Host \$host;
            proxy_set_header X-Real-IP \$remote_addr;
            proxy_set_header X-Forwarded-For \$proxy_add_x_forwarded_for;
            proxy_set_header X-Forwarded-Proto \$scheme;
        }
    }
}
EOF

nginx -g 'daemon off;'
