import React from 'react';
import Button from 'react-bootstrap/Button';
import Card from 'react-bootstrap/Card';
import CardColumns from 'react-bootstrap/CardColumns';
import Col from 'react-bootstrap/Col';
import Row from 'react-bootstrap/Row';
import MdEdit from 'react-icons/lib/md/edit';
import MdPlay from 'react-icons/lib/md/play-arrow';
import MdStop from 'react-icons/lib/md/stop';
import MdSync from 'react-icons/lib/md/sync';
import { Link } from "react-router-dom";
import PlaceHolderImage from '../assets/spagetti-rocket.jpg';


function GameList(props) {
    if (props.loading) {
        return (<div>Loading...</div>);
    }

    return (
        <div>
            <CardColumns>
                {props.games.map(s => <GameCard game={s} key={s.key} {...props} />)}
            </CardColumns>
            <Link to="/new"><Button >Add</Button></Link>
        </div>
    );
}

function GameCard(props) {
    var save = props.game.save;
    var execution = props.game.execution;

    var footer = (
        <Col>
            <Button variant="link" onClick={() => props.onStartInstance(props.game)}><MdPlay size={24} /></Button>
        </Col>);

    var targetVersion = `${save.targetMajorVersion}.${save.targetMinorVersion}.${save.targetPatchVersion ? save.targetPatchVersion : 'x'}`;

    if (execution) {
        footer = (
            <Col>
                <Button variant="link" onClick={() => props.onStopInstance(props.game)}><MdStop size={24} /></Button>
                <Button variant="link" onClick={() => props.onRestartInstance(props.game)}><MdSync size={24} /></Button>
            </Col>);
        var runningVersion = execution.runningVersion;
    }

    // Add save.mods
    // Add save.lastSave
    return (
        <Card style={{ width: '20rem' }}>
            <Card.Img variant="top" src={PlaceHolderImage} />
            <Card.Body>
                <Card.Title>{save.name}</Card.Title>
                <Card.Subtitle>{targetVersion} {runningVersion ? `(Running: ${runningVersion} )` : ''}</Card.Subtitle>
                <Card.Text>{save.description}</Card.Text>
                Connection Info
            </Card.Body>
            <Card.Footer>
                <Row>
                    {footer}
                    <Col>
                        <Link to={`edit/${save.key}`}><Button variant="link"> <MdEdit size={24} /></Button></Link>
                    </Col>
                </Row>
            </Card.Footer>
        </Card>
    );
}

export { GameList };
