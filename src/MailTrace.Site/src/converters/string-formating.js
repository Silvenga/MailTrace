class UpperValueConverter {
    toView(value) {
        return value && value.toUpperCase();
    }
}

class EmailCleanupValueConverter {
    toView(value) {
        if (!value) {
            return;
        }
        if (value.startsWith("<") && value.endsWith(">")) {
            return value.substring(1, value.length - 1);
        }
        return value;
    }
}

class DefaultValueConverter {
    toView(value, defaultStr) {
        return value || defaultStr;
    }
}

export {
UpperValueConverter,
EmailCleanupValueConverter,
DefaultValueConverter
}