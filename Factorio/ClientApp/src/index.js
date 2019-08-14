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
import { requestLoadAllGames } from './services/gameInstances';

const rootElement = document.getElementById('root');

const store = createStore(rootReducer, applyMiddleware(thunkMiddleware));

store.dispatch(beginLoadInstance({}));

requestLoadAllGames().then(data => store.dispatch(onLoadingSuccess(data)));

ReactDOM.render(
    <Provider store={store}>
        <App />
    </Provider>,
    rootElement);

registerServiceWorker();
