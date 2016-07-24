export class App {
    configureRouter(config, router) {
        config.title = 'MailTrace';
        config.map([
            { route: ['', 'emails'], name: 'emails', moduleId: './emails/list', nav: true, title: 'Emails' },
            { route: 'emails/:messageId', name: 'email-details', moduleId: './emails/details', nav: false, title: 'Email Details' }
        ]);

        this.router = router;
    }
}