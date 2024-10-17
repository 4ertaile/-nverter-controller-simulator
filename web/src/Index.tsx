import React, { useEffect } from 'react';
import { makeOnLoad } from './lib';

import useSWR, { mutate } from 'swr';

import styled from 'styled-components';
import { useForm } from 'react-hook-form';

const TextLabel = styled.label`
    font-size: 15px;
    /* font-weight: bold; */
    font-family: 'blankenburg';
`;

const ActionButton = styled.button`
    font-size: 15px;
    font-weight: bold;
    font-family: 'blankenburg';

    &:hover {
        background-color: #dd7311;
    }
`;

const SInput = styled.input`
    font-size: 15px;
    font-family: 'blankenburg';
    font-weight: bold;
    width: 200px;
`;

const Form = styled.form`
    display: flex;
    flex-direction: column;
    align-items: center;
    border: 2px solid #000;
    margin-right: 0.5rem;
    margin-left: 0.5rem;
`;

const FlexRow = styled.div`
    display: inline-flex;
    flex-direction: row;
    align-items: center;
    justify-content: center;
    margin-top: 1rem;
    margin-left: 2rem;
    padding: 1rem;
    hyphens: auto;
`;

const FlexCol = styled.div`
    display: flex;
    flex-direction: column;
    align-items: center;
    justify-content: center;
    margin-top: 1rem;
    margin-left: 2rem;
    padding: 1rem;
`;

const CenteredContainer = styled.div`
    display: flex;
    justify-content: start;
    align-items: center;
    overflow-y: scroll;

    /* height: 100vh; */
    width: 100vw;
`;

type Status = {
    sdStatus: string;
    isWorking: boolean;

    lastUpdateTime: string;
    weatherUpdated: boolean;
    weatherUpdatedStatus: string;

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

type InvertorForm = {
    ip: string;
    port: string;
    id: string;
}

type File = {
    name: string;
    // url: string;
}

type Files = {
    [dir: string]: File[];
}

const FileDisplay = ({ file }: { file: File }) => {
    return (
        <div style={{ margin: '0.5rem 0', padding: '0.5rem', border: '1px solid #bdbdbd', borderRadius: '4px', backgroundColor: '#ffffff' }}>
            <a href={`/file/${file.name}`} style={{ textDecoration: 'none', color: '#5a5a5a', fontWeight: 'bold' }}>
                {file.name}
            </a>
        </div>
    );
}

const FilesDisplay = ({ files }: { files: Files }) => {
    const [selectedDir, setSelectedDir] = React.useState<string | null>(null);

    useEffect(() => {
        if (selectedDir && !Object.keys(files).includes(selectedDir)) {
            setSelectedDir(null);
        }
    },[files]);

    const selectedFiles = selectedDir ? files[selectedDir] : [];

    return (
        <div style={{ textAlign: 'center' }}>
            <div style={{ marginBottom: '1rem' }}>
                <select 
                    onChange={(e) => setSelectedDir(e.target.value)} 
                    value={selectedDir || ''} 
                    style={{ padding: '0.5rem', fontSize: '16px', borderRadius: '4px', border: '1px solid #bdbdbd' }}
                >
                    <option value="" disabled>Select a directory</option>
                    {Object.keys(files).map((dir) => (
                        <option key={dir} value={dir}>{dir}</option>
                    ))}
                </select>
            </div>
            <div style={{ display: 'flex', flexDirection: 'column', alignItems: 'center' }}>
                {selectedFiles.map(
                    (file) => <FileDisplay key={file.name} file={file} />
                )}
            </div>
        </div>
    );
}

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

    const invertorForm = useForm<InvertorForm>({
        defaultValues: {
            ip: '',
            port: '',
            id: ''
        }
    });

    // useSWR with options to disable automatic revalidation
    const { data: opts } = useSWR('/options', fetcher, {
        revalidateOnFocus: false,
        revalidateOnReconnect: false,
        refreshInterval: 0
    });

    // Function to manually revalidate
    const revalidate = () => {
        mutate('/options');
    };

    useEffect(() => {
        (async () => {
            if (!opts) {
                return;
            }

            wifiForm.reset({
                ssid: opts.ssid,
                password: opts.password
            });
            
            weatherForm.reset({
                apiKey: opts.apiKey,
                latitude: opts.latitude,
                longitude: opts.longitude
            });

            invertorForm.reset({
                ip: opts.ip,
                port: opts.port,
                id: opts.id
            });

        })();
    },[opts]);

    const { data: status } = useSWR<Status>('/status', fetcher, { refreshInterval: REFETCH_INTERVAL });

    const { data: files } = useSWR<Files>('/files', fetcher, { refreshInterval: REFETCH_INTERVAL });

    console.log(files);
    
    const send_patch = (url: string) => async () => {
        let response = await fetch(url, {
            method: 'PATCH',
        });

        if (!response.ok) {
            throw new Error('Network response was not ok');
        }
    };

    const send_post = (url: string) => async (data: any) => {
        const queryParams = new URLSearchParams(data).toString();
        const urlWithParams = `${url}?${queryParams}`;

        console.log(data," send to ",urlWithParams);

        let response = await fetch(urlWithParams, {
            method: 'POST',
        });

        if (!response.ok) {
            throw new Error('Network response was not ok');
        }

        revalidate();
    }


    return (
        <CenteredContainer>
            <FlexCol>
                <FlexCol>
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
                        weatherForm.handleSubmit(send_post('/saveWeather'))
                    }>
                        <TextLabel>ApiKey:</TextLabel>
                        <SInput type='text' {...weatherForm.register("apiKey")} /><br />
                        <TextLabel>Latitude:</TextLabel>
                        <SInput {...weatherForm.register("latitude")} /><br />
                        <TextLabel>Longitude:</TextLabel>
                        <SInput {...weatherForm.register("longitude")} /><br />
                        <SInput type='submit' value='Save' />
                    </Form>
                    <Form onSubmit={
                        invertorForm.handleSubmit(send_post('/saveInvertor'))
                    }>
                        <TextLabel>Invertor IP:</TextLabel>
                        <SInput type='text' {...invertorForm.register("ip")} /><br />
                        <TextLabel>Invertor Port:</TextLabel>
                        <SInput type='text' {...invertorForm.register("port")} /><br />
                        <TextLabel>Invertor ID:</TextLabel>
                        <SInput type='text' {...invertorForm.register("id")} /><br />
                        <SInput type='submit' value='Save' />
                    </Form>
                </FlexCol>

                <FlexCol>
                    <FlexCol>
                        <ActionButton onClick={send_patch('/start_ap')}>Start Access Point</ActionButton><br />
                        <ActionButton onClick={send_patch('/connect_wifi')}>Connect to WiFi</ActionButton><br />
                        <ActionButton onClick={send_patch('/sync_time')}>Synchronize Time</ActionButton><br />
                        <ActionButton onClick={send_patch('/start_work')}>Start Work</ActionButton><br />
                        <ActionButton onClick={send_patch('/stop_work')}>Stop Work</ActionButton><br />
                    </FlexCol>
                    {status && (<FlexCol>
                        <TextLabel>SD Status: {status.sdStatus}</TextLabel><br />
                        <TextLabel>SD isWorking: {status.isWorking ? <>Yes</> : <>No</>}</TextLabel><br />
                        <TextLabel>WiFi status: {status.wifiStatus}</TextLabel><br />
                        <TextLabel>Current FileName: {status.currentFileName}</TextLabel><br />
                        <TextLabel>Current fileStatus: {status.fileStatus}</TextLabel><br />
                        <TextLabel>fileParseStatus: {status.fileParseStatus}</TextLabel><br />
                        <TextLabel>Current Time: {status.time}</TextLabel><br />
                        <TextLabel>Temperature: {status.temperature}</TextLabel><br />
                        <TextLabel>Cloudiness: {status.cloudiness}</TextLabel><br />
                        <TextLabel>Last Update Time: {status.lastUpdateTime}</TextLabel><br />
                        <TextLabel>Weather Updated: {status.weatherUpdated}</TextLabel><br />
                        <TextLabel>Weather Updated Status: {status.weatherUpdatedStatus}</TextLabel><br />
                        <TextLabel>Solar Generation: {status.solarGeneration}</TextLabel><br />
                        <TextLabel>Power Consumption: {status.powerConsumption}</TextLabel><br />
                    </FlexCol>)}
                </FlexCol>

                {files &&  <FilesDisplay files={files}/>}
            </FlexCol>
        </CenteredContainer>
    );
};

window.onload = makeOnLoad(App);