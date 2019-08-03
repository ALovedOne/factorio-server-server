import React, { Component } from 'react';
import { GameList } from './GameList';
import { Redirect } from "react-router-dom";


export class Home extends Component {
    static displayName = Home.name;

    constructor(props) {
        super(props);

        this.state = { gameInstances: [], loading: true };

        fetch('api/instances')
            .then(response => response.json())
            .then(data => {
                this.setState((oldState) => {
                    return { gameInstances: data, loading: false }
                });
            });
    }

    onStartInstance(game) {
        this.setState({
            startingInstance: game.key
        });
    }

    onStopInstance(game) {
        fetch("/api/instances/" + game.key + "/stop", {
            method: "POST",
            headers: {
                'Content-Type': 'application/json',
            }
        }).then(resp => this.onStartStop(resp));
    }

    onRestartInstance(game) {
        fetch("/api/instances/" + game.key + "/restart", {
            method: "POST",
            headers: {
                'Content-Type': 'application/json',
            }
        }).then(resp => this.onStartStop(resp));

    }

    onStartStop(response) {
        response.json().then(data => {
            this.setState((oldState) => {
                var newRet = oldState.gameInstances.concat();
                var idx = newRet.findIndex((a) => a.key = data.key);
                newRet[idx] = data;

                return { gameInstances: newRet, loading: false }
            });
        });
    }

    render() {
        if (this.state.startingInstance) {
            return (<Redirect to={`start/?save=${this.state.startingInstance}`} />)
        }

        return (
            <GameList
                loading={this.state.loading}
                games={this.state.gameInstances}
                onStartInstance={(game) => this.onStartInstance(game)}
                onStopInstance={(game) => this.onStopInstance(game)}
                onRestartInstance={(game) => this.onRestartInstance(game)}
            />
        );
    }
}
