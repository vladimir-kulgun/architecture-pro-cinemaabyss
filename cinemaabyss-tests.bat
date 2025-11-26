cls
docker build -t cinemaabyss-tests -f .\tests\postman\Dockerfile .\tests\postman\
docker run cinemaabyss-tests --environment kubernetes