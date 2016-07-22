import {
    HttpClient
} from 'aurelia-fetch-client';

@inject(HttpClient)
export class App {
    constructor(http) {
        http.configure(config => {
            config.withBaseUrl('api/');
        });
        this.http = http;
    }
}