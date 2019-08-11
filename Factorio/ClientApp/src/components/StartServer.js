import React, { Component } from 'react';
import Form from 'react-bootstrap/Form';
import Button from 'react-bootstrap/Button';
import { Redirect } from 'react-router-dom';

// TODO - delete

export class StartServer extends Component {
    constructor(props) {
        super(props);

        let params = new URLSearchParams(props.location.search);

        this.state = {
            imageKey: params.get("save"),
            port: 34197
        }
    }

    handleSubmit(event) {
        event.preventDefault();

        const id = this.state.id;
        const server = this.state.server;



        fetch("api/instances/" + this.state.imageKey + "/start?port=" + this.state.port, {
            method: "POST"
        })
            .then((data) => {
                this.setState({ done: true });
            });
    }

    handleInputChange(event, name) {
        const target = event.target;
        const value = event.type === 'checkbox' ? target.checked : target.value;

        this.setState({
            [name]: value
        });
    }

    render() {
        if (this.state.done) {
            return (<Redirect to="/" />);
        }

        return (
            <div>
                <Form>
                    <Form.Group >
                        <Form.Label>Port</Form.Label>
                        <Form.Control
                            type="text"
                            value={this.state.port}
                            onChange={(event) => this.handleInputChange(event, "port")}
                        />
                    </Form.Group>
                    <Button
                        variant="primary"
                        type="submit"
                        onClick={(event) => this.handleSubmit(event)}
                    >
                        Submit
                    </Button>
                </Form>

                HI: {this.state.imageKey}
                Port: {this.state.port}
            </div>
        );
    }

}