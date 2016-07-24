import { inject, Lazy } from 'aurelia-framework';
import { HttpClient } from 'aurelia-fetch-client';

// polyfill fetch client conditionally
const fetch = !self.fetch ? System.import('isomorphic-fetch') : Promise.resolve(self.fetch);

@inject(Lazy.of(HttpClient))
export class List {

    emails = [];
    params = null;
    to = null;
    from = null;
    before = null;
    after = null;
    page = 1;
    currentPage = 1;
    pageSize = 25;

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

        await this.refresh();
    }

    async refresh() {
        let result = await this.fetchEmails(this.page, this.pageSize, this.before, this.after, this.from, this.to);
        if (this.page != this.currentPage) {
            this.emails.push.apply(this.emails, result.emails);
        } else {
            this.emails = result.emails;
        }
        this.page = result.currentPage;
        this.currentPage = result.currentPage;
    }

    async search() {
        this.page = 1;
        this.currentPage = 1;
        await this.refresh();
    }

    async nextPage() {
        this.page++;
        await this.refresh();
    }

    async fetchEmails(page, pageSize, before, after, from, to) {

        let query = $.param({
            page: page,
            pageSize: pageSize,
            before: before,
            after: after,
            from: from,
            to: to
        });

        let response = await this.http.fetch(`emails?${query}`);
        let result = (await response.json());
        return result;
    }
}