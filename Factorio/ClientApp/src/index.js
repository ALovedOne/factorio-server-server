import 'bootstrap/dist/css/bootstrap.css';
import React from 'react';
import ReactDOM from 'react-dom';
import { Provider } from 'react-redux';
import { applyMiddleware, createStore } from 'redux';
import thunkMiddleware from 'redux-thunk';
import { beginLoadInstance, onLoadingSuccess } from './actions';
import App from './containers/App';
import rootReducer from './reducers';
import registerServiceWorker from './registerServiceWorker';

const rootElement = document.getElementById('root');

const store = createStore(rootReducer, applyMiddleware(thunkMiddleware));

store.dispatch(beginLoadInstance({}));
fetch('api/instances')
    .then(response => response.json())
    .then(data => {
        store.dispatch(onLoadingSuccess(data))
    });

ReactDOM.render(
    <Provider store={store}>
        <App />
    </Provider>,
    rootElement);

registerServiceWorker();
