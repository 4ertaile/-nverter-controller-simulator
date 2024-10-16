import React from 'react';
import { makeOnLoad } from './lib';

import useSWR from 'swr';

import styled from 'styled-components';

const TextLabel = styled.label`
    font-size: 20px;
    font-weight: bold;
`;

const ActionButton = styled.button`
    font-size: 20px;
    font-weight: bold;

    &:hover {
        background-color: #dd7311;
    }
`;

const SInput = styled.input`
    font-size: 20px;
    font-weight: bold;
    width: 200px;
`;

const Form = styled.form`
    display: flex;
    flex-direction: column;
    align-items: center;
`;

const FlexRow = styled.div`
    display: flex;
    flex-direction: row;
    align-items: center;
`;

type Status = {
    sdStatus: string;
    isWorking: boolean;
    wifiStatus: string;
    currentFileName: string;
    fileStatus: string;
    fileParseStatus: string;
    time: string,

    temperature: number,
    cloudiness: number,
    solarGeneration: number,
    powerConsumption: number,
}

type Opts = {
    wifiSSID: string;
    wifiPassword: string;
    apiKey: string;
    latitude: number;
    longitude: number;
}

const fetcher = (url: string) => fetch(url).then(res => res.json());

const App: React.FC = () => {

    const { data: opts } = useSWR<Opts>('/options',fetcher);

    const { data: status } = useSWR<Status>('/status');

    console.log(opts,status);
    const send_patch = (url: string) => async () => {
        let response = await fetch(url, {
            method: 'PATCH',
        });

        if (!response.ok) {
            throw new Error('Network response was not ok');
        }
    };

    return (
        <div>
            {opts && (<FlexRow>
                <Form action='/saveWifi' method='POST'>
                    <TextLabel>WiFi SSID:</TextLabel>
                    <SInput type='text' name='ssid' value={opts.wifiSSID} /><br />
                    <TextLabel>WiFi Password:</TextLabel>
                    <SInput type='password' name='password' value={opts.wifiPassword} /><br />
                    <SInput type='submit' value='Save' />
                </Form>
                <Form action='/saveWeather' method='POST'>
                    <TextLabel>ApiKey:</TextLabel>
                    <SInput type='text' name='apiKey' value={opts.apiKey} /><br />
                    <TextLabel>Latitude:</TextLabel>
                    <SInput name='latitude' value={opts.latitude} /><br />
                    <TextLabel>Longitude:</TextLabel>
                    <SInput name='longitude' value={opts.longitude} /><br />
                    <SInput type='submit' value='Save' />
                </Form>
            </FlexRow>)}

            <FlexRow>
                <div>
                    <ActionButton onClick={send_patch('/start_ap')}>Start Access Point</ActionButton><br />
                    <ActionButton onClick={send_patch('/connect_wifi')}>Connect to WiFi</ActionButton><br />
                    <ActionButton onClick={send_patch('/sync_time')}>Synchronize Time</ActionButton><br />
                    <ActionButton onClick={send_patch('/start_work')}>Start Work</ActionButton><br />
                    <ActionButton onClick={send_patch('/stop_work')}>Stop Work</ActionButton><br />
                    ?????
                    <ActionButton onClick={send_patch('/getFiles')}>getFiles</ActionButton><br />
                </div>
                {status && (<div>
                    <TextLabel>SD Status: {status.sdStatus}</TextLabel><br />
                    <TextLabel>SD isWorking: {status.isWorking}</TextLabel><br />
                    <TextLabel>WiFi status: {status.wifiStatus}</TextLabel><br />
                    <TextLabel>Current FileName: {status.currentFileName}</TextLabel><br />
                    <TextLabel>Current fileStatus: {status.fileStatus}</TextLabel><br />
                    <TextLabel>fileParseStatus: {status.fileParseStatus}</TextLabel><br />
                    
                    <TextLabel>Current Time: {status.time}</TextLabel>
                    <TextLabel>Temperature: {status.temperature}</TextLabel>

                    <TextLabel>Cloudiness: {status.cloudiness}</TextLabel>
                    <TextLabel>Solar Generation: {status.solarGeneration}</TextLabel>
                    <TextLabel>Power Consumption: {status.powerConsumption}</TextLabel>
                </div>)}
            </FlexRow>
        </div>
    );
};

window.onload = makeOnLoad(App);