cls
docker desktop start
minikube start --driver=docker
minikube addons enable metrics-server
minikube dashboard

@pause
minikube delete
docker desktop stop