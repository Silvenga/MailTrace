import {bootstrap} from 'aurelia-bootstrapper-webpack';
import 'bootstrap';

bootstrap(async (aurelia) => {
  aurelia.use
    .standardConfiguration()
    .developmentLogging();

  const rootElement = document.body;
  rootElement.setAttribute('aurelia-app', '');

  await aurelia.start();
  aurelia.setRoot('app', rootElement);

});
