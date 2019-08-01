import React, { Component } from 'react';
import Button from 'react-bootstrap/Button';
import Card from 'react-bootstrap/Card';
import CardColumns from 'react-bootstrap/CardColumns';
import MdEdit from 'react-icons/lib/md/edit';
import MdPlay from 'react-icons/lib/md/play-arrow';
import MdStop from 'react-icons/lib/md/stop';
import MdSync from 'react-icons/lib/md/sync';
import { Link, Redirect } from "react-router-dom";
import PlaceHolderImage from '../assets/spagetti-rocket.jpg';
import Row from 'react-bootstrap/Row'
import Col from 'react-bootstrap/Col'


export class Home extends Component {
    static displayName = Home.name;

    constructor(props) {
        super(props);

        this.state = { servers: [], loading: true, addNew: false };

        fetch('api/instances')
            .then(response => response.json())
            .then(data => {
                this.setState({ servers: data, loading: false });
            });
        fetch('api/executions')
            .then(response => response.json())
            .then(data => {
                this.setState({ executions: data, loadingExecutions: false });
            });
    }

    addServer() {
        this.setState({ "addNew": true });
    }

    static renderServersTable(servers) {
        return (
            <CardColumns>
                {
                    servers.map(s =>
                        <Card style={{ width: '20rem' }} key={s.key}>
                            <Card.Img variant="top" src={PlaceHolderImage} />
                            <Card.Body>
                                <Card.Title>{s.name}</Card.Title>
                                <Card.Subtitle>0.17.x</Card.Subtitle>
                                <Card.Text>{s.description}</Card.Text>
                                Connection Info
                            </Card.Body>
                            <Card.Footer>
                                <Row>
                                    <MdPlay size={26} />
                                    <MdStop size={26} />
                                    <Link to={`edit/${s.key}`}><MdEdit size={24} /></Link>
                                    <MdSync size={26} />
                                </Row>
                            </Card.Footer>
                        </Card>
                    )
                }
            </CardColumns>
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
