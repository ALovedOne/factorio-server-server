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

        this.state = { servers: [], loading: true };

        fetch('api/instances')
            .then(response => response.json())
            .then(data => {
                this.setState({ servers: data, loading: false });
            });
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
                                    <Col>Col 1</Col>
                                    <Col>
                                        <Button variant="link"> <MdPlay size={24} /></Button>
                                        <Button variant="link"><MdStop size={24} /></Button>
                                        <Button variant="link"><MdSync size={24} /></Button>
                                        <Link to={`edit/${s.key}`}><Button variant="link"> <MdEdit size={24} /></Button></Link>
                                    </Col>
                                </Row>
                            </Card.Footer>
                        </Card>
                    )
                }
            </CardColumns>
        );
    }

    render() {
        let contents = this.state.loading
            ? <p><em>Loading...</em></p>
            : Home.renderServersTable(this.state.servers);

        return (
            <div>
                {contents}
                <Link to="/new"><Button >Add</Button></Link>
            </div>
        );
    }
}
