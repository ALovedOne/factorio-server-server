import React, { Component } from 'react';
import Form from 'react-bootstrap/Form';
import Button from 'react-bootstrap/Button';
import { Redirect } from 'react-router-dom';

export class EditServer extends Component {
    constructor(props) {
        super(props);

        let id = props.match.params.id;


        if (id) {
            this.state = { id: id, loading: true, done: false };

            fetch(`api/instance/${id}`)
                .then(response => response.json())
                .then(data => {
                    this.setState({ id: this.state.id, server: data, loading: false })
                });
        } else {
            this.state = {
                id: id, loading: false, done: false, server: {}
            };
        }
    }

    handleSubmit(event) {
        event.preventDefault();

        const id = this.state.id;
        const server = this.state.server;

        const url = id ? `api/instance/${id}` : `api/instance`;
        const method = id ? 'PUT' : 'POST';


        fetch(url, {
            method: method,
            headers: {
                'Content-Type': 'application/json',
            },
            body: JSON.stringify(server)
        })
            .then((data) => {
                this.setState({ done: true });
            });
    }

    handleInputChange(event, name) {
        const target = event.target;
        const value = event.type === 'checkbox' ? target.checked : target.value;

        const server = Object.assign({}, this.state.server, { [name]: value });
        // Partial update
        this.setState({
            server: server
        });
    }

    // TODO - validate version
    // TODO - mods
    renderEditForm(server) {
        return (
            <div>
                <Form>
                    <Form.Group controlId="frmEditServer">
                        <Form.Label>Server Name</Form.Label>
                        <Form.Control
                            type="text"
                            value={server.name}
                            onChange={(event) => this.handleInputChange(event, "name")}
                        />
                    </Form.Group>
                    <Form.Group controlId="frmEditServer">
                        <Form.Label>Description</Form.Label>
                        <Form.Control
                            type="text"
                            value={server.description}
                            onChange={(event) => this.handleInputChange(event, "description")}
                        />
                    </Form.Group>
                    <Form.Control
                        placeholder="major"
                        value={server.targetMajorVersion}
                        onChange={(event) => this.handleInputChange(event, "targetMajorVersion")}
                    />
                    <Form.Control
                        placeholder="minor"
                        value={server.targetMinorVersion}
                        onChange={(event) => this.handleInputChange(event, "targetMinorVersion")}
                        disabled={!server.targetMajorVersion}
                    />
                    <Button
                        variant="primary"
                        type="submit"
                        onClick={(event) => this.handleSubmit(event)}
                    >
                        Submit
                    </Button>
                </Form>
            </div >
        )
    }

    render() {
        if (this.state.done) {
            return (
                <Redirect to="/" />
            )
        }
        let content = this.state.loading
            ? <p><em>Loading...</em></p>
            : this.renderEditForm(this.state.server);

        return (
            <div>
                <h1> Editing: {this.state.id}</h1>
                {content}
            </div>
        );
    }
}