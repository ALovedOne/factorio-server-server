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
    var game = props.game;
    var execution = props.game.currentExecution;

    var footer = (
        <Col>
            <Button variant="link" onClick={() => props.onStartInstance(props.game)}><MdPlay size={24} /></Button>
        </Col>);

    var targetVersion = formatVersion(game.targetVersion);

    if (execution) {
        footer = (
            <Col>
                <Button variant="link" onClick={() => props.onStopInstance(props.game)}><MdStop size={24} /></Button>
                <Button variant="link" onClick={() => props.onRestartInstance(props.game)}><MdSync size={24} /></Button>
            </Col>);
        var runningVersion = execution.runningVersion;
    }

    if (game.lastSave) {
        var saveVersion = formatVersion(game.lastSave.version); 
    }

    // Add save.mods
    return (
        <Card >
            <Card.Img variant="top" src={PlaceHolderImage} />
            <Card.Body>
                <Card.Title>{game.name}</Card.Title>
                <Card.Subtitle>{targetVersion} {runningVersion ? `(Running: ${runningVersion})` : saveVersion ? `(Last Save: ${saveVersion})` : ""}</Card.Subtitle>
                <Card.Text>
                    {game.description}
                </Card.Text>
                <Card.Text>
                    Mod Count: {game.mods.length}
                </Card.Text>
                <Card.Text>
                    Connection Info
                </Card.Text>
            </Card.Body>
            <Card.Footer>
                <Row>
                    {footer}
                    <Col>
                        <Link to={`edit/${game.key}`}><Button variant="link"> <MdEdit size={24} /></Button></Link>
                    </Col>
                </Row>
            </Card.Footer>
        </Card >
    );
}

function formatVersion(version) {
    if (version.patch) {
        return `${version.major}.${version.minor}.${version.patch}`;
    } else {
        return `${version.major}.${version.minor}x`;
    }
}

export { GameList };