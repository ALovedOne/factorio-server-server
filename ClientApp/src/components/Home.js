import React, { Component } from 'react';
import { BrowserRouter as Router, Route, Link, Redirect } from "react-router-dom";
import Button from 'react-bootstrap/Button';

export class Home extends Component {
    static displayName = Home.name;

    constructor(props) {
        super(props);

        this.state = { servers: [], loading: true, addNew: false };

        fetch('api/server')
            .then(response => response.json())
            .then(data => {
                this.setState({ servers: data, loading: false })
            });
    }

    addServer() {
        this.setState({ "addNew": true });
    }

    static renderServersTable(servers) {
        return (
            <table className='table table-striped'>
                <thead>
                    <tr>
                        <th>Name</th>
                        <th>Port</th>
                        <th>Version</th>
                        <th>Last Save</th>
                        <th>Description</th>
                        <th></th>
                    </tr>
                </thead>
                <tbody>
                    {servers.map(s =>
                        <tr key={s.slug}>
                            <td>{s.name}</td>
                            <td>{s.port}</td>
                            <td>{s.targetMajorVersion}.{s.targetMinorVersion ? s.targetMinorVersion : 'x'}</td>
                            <td>{s.lastSaveMajorVersion ? `${s.lastSaveMajorVersion}.${s.lastSaveMinorVersion}` : ''}</td>
                            <td>{s.description}</td>
                            <td>
                                <Link to={`edit/${s.slug}`}>Edit</Link>
                            </td>
                        </tr>
                    )}
                </tbody>
            </table>
        );
    }

    render() {
        if (this.state.addNew) {
            return (
                <Redirect to="/new" />
            )
        }
        let contents = this.state.loading
            ? <p><em>Loading...</em></p>
            : Home.renderServersTable(this.state.servers);

        return (
            <div>
                {contents}
                <Button onClick={() => this.addServer()} >
                    Add
                    </Button>
            </div>
        );
    }
}
