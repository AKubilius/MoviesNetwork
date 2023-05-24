import React from 'react'
import { useEffect, useState } from 'react'
import axios, { AxiosRequestConfig } from 'axios'
import Grid from '@mui/material/Grid/Grid';
import Request from './Request';
import Box from '@mui/material/Box/Box';

interface RequestsProps {
    handleClos: () => void;
  }


export const Requests : React.FC<RequestsProps> = ({ handleClos }) => {

    const [users, setusers] = useState<any>([]);
    const token = `Bearer ${sessionStorage.getItem("token")}`
    const fetch = async () => {

        const { data } = await axios.get(`https://localhost:7019/FriendRequest`,
            {
                headers: {
                    'Content-Type': 'application/json',
                    Accept: 'application/json',
                    Authorization: token
                }
            })
        setusers(data)
        console.log(data)

    }
    useEffect(() => {
        fetch()
    }, [])

//Requests
    return (
        <div>

            {users?.slice(0, 3).map((user: any, index: React.Key | null | undefined) => (
                <Request
                    username={user.name}
                    id={user.id}
                    handleClose={handleClos}
                />))}
        </div>
    )
}
