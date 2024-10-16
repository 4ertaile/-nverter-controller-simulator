import React from 'react';
import { makeOnLoad } from './lib';

import styled from 'styled-components';

type Status = {
    files: {
        fname: string,
        date: string
    }[],
    time: string
};

const App: React.FC = () => {

    const [status,setStatus] = React.useState<Status | null>(null);

    (async () => {
        let resp = await fetch('/getFiles', {
            method: 'GET',
            headers: {
                'Content-Type': 'application/json',
            },
        });
        let data = await resp.json();
        setStatus(data);
    })();

    return (
        <div>
            <h1>ESP32 SD Card Status</h1>
            {/* <p>SD Card Status: {SD_Status}</p> */}
            {status && (<>
                <h2>Files for {status.time}:</h2>
                <ul>
                    {
                    status.files.filter(({ date }) => {
                        const fileDate = new Date(date).toDateString();
                        const todayDate = new Date().toDateString();
                        return fileDate === todayDate;
                    }).map((file) => (
                        <li key={file.fname}>{file.fname}</li>
                    ))
                    }
                </ul>
                <h2>Files for last 25 days:</h2>
                <ul>
                    {status.files.map(file => (
                        <li key={file.fname}>{file.fname}</li>
                    ))}
                </ul>
            </>)}
        </div>
    );
}

window.onload = makeOnLoad(App);