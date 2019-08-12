import React from 'react';
import Navbar from 'react-bootstrap/Navbar';
import EditGameForm from '../containers/EditGameForm';
import GameList from '../containers/GameList';
import LaunchGameForm from '../containers/LaunchGameForm';
import Button from 'react-bootstrap/Button';


export const Home = ({ editingGame, launchingGame, onAddNew }) => {
    if (editingGame) {
        var content = (<EditGameForm />)
    } else if (launchingGame) {
        var content = (<LaunchGameForm />);
    } else {
        var content = (<GameList />);
    }

    return (
        <div>
            <Navbar bg="dark" variant="dark">
                <Navbar.Brand>Factorio!-server-server</Navbar.Brand>
            </Navbar>
            <Button onClick={() => onAddNew()}>Add</Button>
            {content}
        </div>
    )
};

export default Home