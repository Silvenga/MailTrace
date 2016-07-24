import { inject, Lazy } from 'aurelia-framework';
import { HttpClient } from 'aurelia-fetch-client';

const fetch = !self.fetch ? System.import('isomorphic-fetch') : Promise.resolve(self.fetch);

@inject(Lazy.of(HttpClient))
export class EmailDetails {

    constructor(getHttpClient) {
        this.getHttpClient = getHttpClient;
    }

    async activate(params) {

        if (!params.messageId) {
            return;
        }

        this.params = params;

        await fetch;
        this.http = this.getHttpClient();

        this.http.configure(config => {
            config
                .useStandardConfiguration()
                .withBaseUrl('/api/');
        });

        await this.refresh();
    }

    async refresh() {
        this.details = null;
        this.details = await this.fetchEmailDetails(this.params.messageId);
    }

    async fetchEmailDetails(messageId) {
        let response = await this.http.fetch(`emails/${messageId}`);
        let email = (await response.json());
        return email;
    }

    dsnToClass(dsnCode) {
        let panelClass = "default";

        if (dsnCode && dsnCode.startsWith("2")) {
            panelClass = "success";
        }

        if (dsnCode && dsnCode.startsWith("5")) {
            panelClass = "danger";
        }

        if (dsnCode && dsnCode.startsWith("4")) {
            panelClass = "warning";
        }

        if (!dsnCode) {
            panelClass = "info";
        }

        return `panel-${panelClass}`;
    }
}