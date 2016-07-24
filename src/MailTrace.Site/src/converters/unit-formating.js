class HumanBytesValueConverter {
    toView(value) {
        return value && this.formatBytes(value);
    }

    // http://stackoverflow.com/a/18650828/2001966
    formatBytes(bytes, decimals = 1) {
        if (bytes == 0) return '0 Bytes';
        const k = 1024;
        const dm = decimals + 1 || 3;
        const sizes = ['Bytes', 'KB', 'MB', 'GB', 'TB', 'PB', 'EB', 'ZB', 'YB'];
        const i = Math.floor(Math.log(bytes) / Math.log(k));
        return parseFloat((bytes / Math.pow(k, i)).toFixed(dm)) + ' ' + sizes[i];
    }
}

export {
HumanBytesValueConverter
}