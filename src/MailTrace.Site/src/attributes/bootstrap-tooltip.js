import {customAttribute, inject} from 'aurelia-framework';
import $ from 'jquery';
import 'bootstrap';

// https://github.com/aurelia/templating/issues/233

@customAttribute('bootstrap-tooltip')
@inject(Element)
export class BootstrapTooltip {
    constructor(element) {
        this.element = element;
    }

    bind() {
        $(this.element).tooltip();
    }

    unbind() {
        $(this.element).tooltip('destroy');
    }
}