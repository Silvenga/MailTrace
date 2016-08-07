import { inject, Lazy } from 'aurelia-framework';
import { HttpClient } from 'aurelia-fetch-client';
import {Router} from 'aurelia-router';
import Moment from 'moment';

// polyfill fetch client conditionally
const fetch = !self.fetch ? System.import('isomorphic-fetch') : Promise.resolve(self.fetch);

@inject(Lazy.of(HttpClient), Router)
export class List {

    emails = [];
    params = {};
    to;
    from;
    before;
    after;
    page;
    pageSize;

    constructor(getHttpClient, router) {
        this.getHttpClient = getHttpClient;
        this.router = router;
        this.that = this;
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

        var query = {
            to: this.to,
            from: this.from,
            before: this.before,
            after: this.after,
            page: this.page,
            pageSize: this.pageSize
        }
        this.router.navigateToRoute('emails', query, { replace: true });

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

        // let response = await this.http.fetch(`emails?${query}`);
        // let result = await response.json();
        let result = this.createFixtures(page, pageSize);
        return result;
    }

    createFixtures(page, pageSize) {
        console.log(`Openning: ${page}`);
        let array = [];
        for (var index = 0; index < pageSize; index++) {
            array.push({
                to: `<message-${index}-page-${page}@silvenga.com>`,
                from: "<from@silvenga.com>",
                firstSeen: Moment.now(),
                messageId: `${index}`
            });
        }

        const count = 100;

        return {
            emails: array,
            page: page,
            pageSize: pageSize,
            count: count
        };
    }

    async getPage(page, pageSize) {
        this.page = page;
        this.pageSize = pageSize;
        await this.refresh();
    }
}