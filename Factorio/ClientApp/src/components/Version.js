import React from 'react';
import { connect } from 'react-redux';

function Version({ version }) {
    if (version.patch) {
        return (<span>{`${version.major}.${version.minor}.${version.patch}`}</span>);
    } else {
        return (<span>{`${version.major}.${version.minor}x`}</span>);
    }
}

export default connect()(Version);