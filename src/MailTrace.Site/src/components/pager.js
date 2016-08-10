import {bindable, bindingMode} from 'aurelia-framework';

// HACK
@(a => a) class A { }

export class Pager {

    @bindable({ defaultBindingMode: bindingMode.twoWay }) page;
    @bindable({ defaultBindingMode: bindingMode.twoWay }) pageSize;
    @bindable({ defaultBindingMode: bindingMode.twoWay }) count;
    @bindable({ defaultBindingMode: bindingMode.twoWay }) getPage;
    @bindable allowDestruction = false;

    _jumpPage = -1;
    get jumpPage() {
        return this._jumpPage === -1 ? this.page : this._jumpPage;
    }
    set jumpPage(value) {
        this._jumpPage = value;
    }

    get canJump(){
        return this.jumpPage > 0 && this.jumpPage <= this.pages
    }

    constructor() {
    }

    async activate() {
        this.getCurrentPage();
    }

    get pages() {
        let pages = Math.ceil(this.count / this.pageSize);
        return pages;
    }

    next() {
        if (this.canNext) {
            this.page++;
            this.getCurrentPage();
        }
    }

    prev() {
        if (this.canPrev) {
            this.page--;
            this.getCurrentPage();
        }
    }

    getCurrentPage() {
        this.jumpPage = this.page;
        this.getPage.getPage(this.page, this.pageSize);
    }

    get canNext() {
        return this.page < this.pages;
    }

    get canPrev() {
        return this.page > 1;
    }

    jump() {
        console.log("Jump: " + this.jumpPage);
        if (this.canJump) {
            this.page = this.jumpPage;
            this.getCurrentPage();
        }
    }

}