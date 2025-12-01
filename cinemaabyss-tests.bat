cls
rem docker build -t cinemaabyss-tests -f .\tests\postman\Dockerfile .\tests\postman\
rem docker run --add-host cinemaabyss.example.com:172.17.0.1 cinemaabyss-tests --environment local

npm run test:kubernetes
