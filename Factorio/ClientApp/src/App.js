import React, { Component } from 'react';
import { Route } from 'react-router';
import { Layout } from './components/Layout';
import { Home } from './components/Home';
import { EditServer } from './components/EditServer';
import { StartServer } from './components/StartServer';

export default class App extends Component {
    static displayName = App.name;

    render() {
        return (
            <Layout>
                <Route exact path='/' component={Home} />
                <Route path='/edit/:id' component={EditServer} />
                <Route path='/new' component={EditServer} />
                <Route path='/start/' component={StartServer}/>
            </Layout>
        );
    }
}
