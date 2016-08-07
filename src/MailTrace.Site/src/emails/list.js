import { inject, Lazy } from 'aurelia-framework';
import { HttpClient } from 'aurelia-fetch-client';
import {Router} from 'aurelia-router';

// polyfill fetch client conditionally
const fetch = !self.fetch ? System.import('isomorphic-fetch') : Promise.resolve(self.fetch);

@inject(Lazy.of(HttpClient), Router)
export class List {

    constructor(getHttpClient, router) {
        this.getHttpClient = getHttpClient;
        this.router = router;
    }

    async activate(params) {

        await fetch;
        this.http = this.getHttpClient();

        this.http.configure(config => {
            config
                .useStandardConfiguration()
                .withBaseUrl('/api/');
        });

        this.populateFromParams(params);

        await this.refresh();
    }

    populateFromParams(params) {
        this.params = params;
        this.to = params.to;
        this.from = params.from;
        this.before = params.before;
        this.after = params.after;
        this.page = params.page || 1;
        this.pageSize = params.pageSize || 25;
    }

    async refresh() {
        let result = await this.fetchEmails(
            this.page,
            this.pageSize,
            this.before,
            this.after,
            this.from,
            this.to);
        this.emails = result.emails;
        this.page = result.page;
        this.pageSize = result.pageSize;
        this.count = result.count;
        this.calculatePaging();
    }

    calculatePaging() {
        let pages = Math.ceil(this.count / this.pageSize);

        this.pagination = {
            pageSize: this.pageSize,
            page: this.page,
            count: this.count,
            pages: pages
        };
        console.log(this.pagination);
    }

    async search() {
        var query = {
            to: this.to,
            from: this.from,
            before: this.before,
            after: this.after,
            page: this.page,
            pageSize: this.pageSize
        }
        this.router.navigateToRoute('emails', query, { replace: true });
        this.refresh();
    }

    async nextPage() {
        this.page++;
        search();
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

    async checkForMore(topIndex, isAtBottom, isAtTop) {
        if(isAtBottom){
            nextPage();
        }
    }
}