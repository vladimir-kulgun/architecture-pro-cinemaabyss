cls
kubectl apply -f src/kubernetes/namespace.yaml

kubectl apply -f src/kubernetes/configmap.yaml
kubectl apply -f src/kubernetes/secret.yaml
kubectl apply -f src/kubernetes/dockerconfigsecret.yaml
kubectl apply -f src/kubernetes/postgres-init-configmap.yaml

kubectl apply -f src/kubernetes/postgres.yaml
@pause
kubectl apply -f src/kubernetes/kafka/kafka.yaml
@pause
kubectl apply -f src/kubernetes/monolith.yaml
@pause
kubectl apply -f src/kubernetes/movies-service.yaml
@pause
kubectl apply -f src/kubernetes/events-service.yaml
@pause
kubectl apply -f src/kubernetes/events-consumer.yaml
@pause
kubectl apply -f src/kubernetes/proxy-service.yaml
@pause

minikube addons enable ingress
kubectl apply -f src/kubernetes/ingress.yaml
minikube tunnel