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

            fetch(`api/instances/${id}`)
                .then(response => response.json())
                .then(data => {
                    this.setState({ id: this.state.id, server: data, loading: false })
                });
        } else {
            this.state = {
                id: id, loading: false, done: false, server: { targetVersion: {} }
            };
        }
    }

    handleSubmit(event) {
        event.preventDefault();

        const id = this.state.id;
        const server = this.state.server;

        const url = id ? `api/instances/${id}` : `api/instances`;
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

    handleInputChange(name, value) {
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
                            onChange={(event) => this.handleInputChange("name", event.target.value)}
                        />
                    </Form.Group>
                    <Form.Group controlId="frmEditServer">
                        <Form.Label>Description</Form.Label>
                        <Form.Control
                            type="text"
                            value={server.description}
                            onChange={(event) => this.handleInputChange("description", event.target.value)}
                        />
                    </Form.Group>
                    <VersionEditor
                        version={server.targetVersion}
                        onChange={(event, version) => this.handleInputChange("targetVersion", version)}
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

class VersionEditor extends Component {
    handleInputChange(event, field) {
        var newVersion = Object.assign({}, this.props.version, { [field]: event.target.value });
        this.props.onChange(event, newVersion);
    }

    render() {
        let props = this.props;

        return < Form.Group >
            <Form.Label>Target Version</Form.Label>
            <Form.Control
                placeholder="major"
                value={props.version.major}
                onChange={(event) => this.handleInputChange(event, "major")}
            />
            <Form.Control
                placeholder="minor"
                value={props.version.minor}
                onChange={(event) => this.handleInputChange(event, "minor")}
                disabled={props.version.major == null}
            />
            <Form.Control
                placeholder="patch"
                value={props.version.patch}
                onChange={(event) => this.handleInputChange(event, "patch")}
                disabled={props.version.minor == null}
            />
        </Form.Group >;
    }
}