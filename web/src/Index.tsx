import React, { useEffect } from 'react';
import { makeOnLoad } from './lib';

import useSWR from 'swr';

import styled from 'styled-components';
import { useForm } from 'react-hook-form';

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
    margin-top: 1rem;
    margin-left: 2rem;
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

type WifiForm = {
    ssid: string;
    password: string;
}

type WeatherForm = {
    apiKey: string;
    latitude: string;
    longitude: string;
}

type Files = {
    name: string;
}[]

const REFETCH_INTERVAL = 2000; //ms

const fetcher = (url: string) => fetch(url).then(res => res.json());

const App: React.FC = () => {

    const wifiForm = useForm<WifiForm>({
        defaultValues: {
            ssid: '',
            password: ''
        }
    });

    const weatherForm = useForm<WeatherForm>({
        defaultValues: {
            apiKey: '',
            latitude: '',
            longitude: ''
        }
    });

    useEffect(() => {
        (async () => {
            let response = await fetch('/options');
            let data = await response.json();

            wifiForm.reset({
                ssid: data.ssid,
                password: data.password
            });
            weatherForm.reset({
                apiKey: data.apiKey,
                latitude: data.latitude,
                longitude: data.longitude
            });
        })();
    },[]);

    const { data: status } = useSWR<Status>('/status', fetcher, { refreshInterval: REFETCH_INTERVAL });

    //todo: fix this
    const { data: files } = useSWR<Files>('/files', fetcher, { refreshInterval: REFETCH_INTERVAL });

    
    const send_patch = (url: string) => async () => {
        let response = await fetch(url, {
            method: 'PATCH',
        });

        if (!response.ok) {
            throw new Error('Network response was not ok');
        }
    };

    //todo: fix this
    const send_post = (url: string) => async (data: any) => {
        console.log(data," send to ",url);

        const queryParams = new URLSearchParams(data).toString();
        const urlWithParams = `${url}?${queryParams}`;

        let response = await fetch(urlWithParams, {
            method: 'POST',
        });

        if (!response.ok) {
            throw new Error('Network response was not ok');
        }
    }


    return (
        <div>
            <FlexRow>
                <Form onSubmit={
                    wifiForm.handleSubmit(send_post('/saveWifi'))
                }>
                    <TextLabel>WiFi SSID:</TextLabel>
                    <SInput type='text' {...wifiForm.register("ssid")} /><br />
                    <TextLabel>WiFi Password:</TextLabel>
                    <SInput type='password' {...wifiForm.register("password")} /><br />
                    <SInput type='submit' value='Save' />
                </Form>
                <Form onSubmit={
                    wifiForm.handleSubmit(send_post('/saveWeather'))
                }>
                    <TextLabel>ApiKey:</TextLabel>
                    <SInput type='text' {...weatherForm.register("apiKey")} /><br />
                    <TextLabel>Latitude:</TextLabel>
                    <SInput {...weatherForm.register("latitude")} /><br />
                    <TextLabel>Longitude:</TextLabel>
                    <SInput {...weatherForm.register("longitude")} /><br />
                    <SInput type='submit' value='Save' />
                </Form>
            </FlexRow>

            <FlexRow>
                <div>
                    <ActionButton onClick={send_patch('/start_ap')}>Start Access Point</ActionButton><br />
                    <ActionButton onClick={send_patch('/connect_wifi')}>Connect to WiFi</ActionButton><br />
                    <ActionButton onClick={send_patch('/sync_time')}>Synchronize Time</ActionButton><br />
                    <ActionButton onClick={send_patch('/start_work')}>Start Work</ActionButton><br />
                    <ActionButton onClick={send_patch('/stop_work')}>Stop Work</ActionButton><br />
                </div>
                {status && (<div>
                    <TextLabel>SD Status: {status.sdStatus}</TextLabel><br />
                    <TextLabel>SD isWorking: {status.isWorking}</TextLabel><br />
                    <TextLabel>WiFi status: {status.wifiStatus}</TextLabel><br />
                    <TextLabel>Current FileName: {status.currentFileName}</TextLabel><br />
                    <TextLabel>Current fileStatus: {status.fileStatus}</TextLabel><br />
                    <TextLabel>fileParseStatus: {status.fileParseStatus}</TextLabel><br />
                    <TextLabel>Current Time: {status.time}</TextLabel><br />
                    <TextLabel>Temperature: {status.temperature}</TextLabel><br />
                    <TextLabel>Cloudiness: {status.cloudiness}</TextLabel><br />

                    <TextLabel>Solar Generation: {status.solarGeneration}</TextLabel><br />
                    <TextLabel>Power Consumption: {status.powerConsumption}</TextLabel><br />
                </div>)}

                {files && 
                    <FlexRow>{
                        files.map(
                            ({name}) => (<React.Fragment key={name}>
                                <TextLabel>{name}</TextLabel><br />
                            </React.Fragment>)
                        )    
                    }</FlexRow>
                }
            </FlexRow>
        </div>
    );
};

window.onload = makeOnLoad(App);