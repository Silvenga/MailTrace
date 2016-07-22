
import './styles/styles.css';
import '../node_modules/bootstrap/dist/css/bootstrap.css'
import 'bootstrap'

require.ensure(["./bootstrapper"], function(require) {
    require("./bootstrapper");
});