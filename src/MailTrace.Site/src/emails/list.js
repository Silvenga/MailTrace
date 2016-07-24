import { inject, Lazy } from 'aurelia-framework';
import { HttpClient } from 'aurelia-fetch-client';

// polyfill fetch client conditionally
const fetch = !self.fetch ? System.import('isomorphic-fetch') : Promise.resolve(self.fetch);

@inject(Lazy.of(HttpClient))
export class List {

    constructor(getHttpClient) {
        this.getHttpClient = getHttpClient;
    }

    async activate() {

        await fetch;
        this.http = this.getHttpClient();

        this.http.configure(config => {
            config
                .useStandardConfiguration()
                .withBaseUrl('/api/');
        });

        this.emails = await this.fetchEmails();
    }

    async fetchEmails(page = 1, pageSize = 25, before = null, after = null, from = null, to = null) {

        let query = $.param({
            page: page,
            pageSize: pageSize,
            before: before,
            after: after,
            from: from,
            to: to
        });

        let response = await this.http.fetch(`emails?${query}`);
        let emails = (await response.json()).logs;
        return emails;
    }
}