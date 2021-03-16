var configuration_file = require('./config.json');
var bodyParser = require('body-parser');

//создаем Express приложение
var express = require('express');
var application = express();

//устанавливаем view для index.ejs и слушаем порт
application.set('view engine', 'ejs');
application.listen(configuration_file.port);

console.log("Локальный сервер находится по адресу http://" + configuration_file.host + ":" + configuration_file.port);

//определяем, что по адресу / будет index.html
application.get('/', function(request, return_result){
    return_result.sendFile(__dirname + "/index.html");
});

const gRPC = require('grpc');
const proto_loader = require('@grpc/proto-loader');
const { getType } = require('mime');

//описываем настройки для proto
const PROTO_PATH = __dirname + '/CatInfoService.proto';
const packageDefinition = proto_loader.loadSync(
  PROTO_PATH,
  {
    defaults: true,
    oneofs: true,
    keepCase: true,
    enums: String,
    longs: String
    
  });
const catInfo_proto = gRPC.loadPackageDefinition(packageDefinition);
const client = new catInfo_proto.CatInfoService(configuration_file.address, gRPC.credentials.createInsecure());

var URL_encoded_parser = bodyParser.urlencoded({ extended: false })
application.post('/index', URL_encoded_parser, function (request, return_result) {
    //в случае если не можем обработать запрос
    if (!request.body) return return_result.sendStatus(400);

    //обрабатываем запрос

    //создаем объект нашего класса
    var cat = {
        name: request.body.cat_name,
        age: parseInt(request.body.cat_age),
        kind: request.body.cat_kind,
        sex: request.body.cat_sex
    }

    //отправляем его серверу
     client.getCatInfo(
        cat,
        function(error, response) {
          if (error) console.log('error ', error);
          else
          console.log("Ответ от сервера:", response.output_text);
          return return_result.render('index', {cat_info_output : response.output_text});
        }
    )
    
});