import moment from 'moment';

class LongTimeFormatValueConverter {
    toView(value) {
        return moment(value).format('M/D/YYYY h:mm:ss a');
    }
}

class TimeAgoFormatValueConverter {
    toView(value) {
        return moment(value).fromNow();
    }
}

class DurationFormatValueConverter {
    toView(value, metric) {
        console.log(value);
        return moment.duration(parseInt(value), metric).humanize();
    }
}

export {
LongTimeFormatValueConverter,
TimeAgoFormatValueConverter,
DurationFormatValueConverter
}