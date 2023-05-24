import React, { useState, useEffect, useCallback, useRef  } from 'react';
import InfiniteScroll from 'react-infinite-scroll-component';
import * as signalR from '@microsoft/signalr';
import { useParams } from 'react-router-dom';
import './Chat.css'
import Box from '@mui/material/Box/Box';
import { ChatMovie } from './ChatMovie';
import { getRequest } from '../Api/Api';
import User from '../Users/User';

interface User{
  userName: string;
}
interface Movie {
  title: string;
  posterUrl: string;
}

interface Message {
  id:number;
  sender: User;
  body: string;
  image64: string;
  isMovie: boolean;
  movie?: Movie;
  dateTime:Date;
}

interface ChatProps {
  connection: signalR.HubConnection | null;
  setConnection: (connection: signalR.HubConnection | null) => void;
  roomID: string | undefined;
}

const token = `Bearer ${sessionStorage.getItem("token")}`;

const Chat: React.FC<ChatProps> = ({
  connection,
  setConnection,
  roomID,
}) => {
  const userName = `${sessionStorage.getItem("name")}`;
  const [messages, setMessages] = useState<Message[]>([]);
  const [user, setUser] = useState([]);
  

  useEffect(() => {
    const fetchData = async () => {
      const data = await getRequest('https://localhost:7019/User/current', '' );
      setUser(data.userName);
      
    };
    fetchData();
    
  }, []);

  useEffect(() => {
    const newConnection = new signalR.HubConnectionBuilder()
      .withUrl(`https://localhost:7019/chatHub/?paramName1=${userName}`, {
        accessTokenFactory: () => token,
      })
      .withAutomaticReconnect()
      .build();

    setConnection(newConnection);
  }, []);

useEffect(() => {
  if (connection) {
    connection
      .start()
      .then(() => {
        connection.on('ReceiveMessage', async (id: number, sender: User, body: string, image64: string, isMovie: boolean, dateTime: Date) => {
          setMessages((prevMessages) => [
            ...prevMessages,
            { id, sender: { ...sender, userName: sender.userName || userName }, body, image64, isMovie, dateTime },
          ]);
        });
      })
      .catch((e) => console.log('Connection failed: ', e));
  }
}, [connection]);



const joinRoom = async (roomIdToJoin: string) => {
  if (connection && connection.state === signalR.HubConnectionState.Connected) {
    await connection.send('JoinUserRooms', roomIdToJoin);
    

  }
};
useEffect(() => {
  if (connection && roomID) {
    joinRoom(roomID);
    fetchMessages(roomID);
  }
}, [connection, roomID]);

const fetchMessages = async (roomID: string) => {
  if (connection && roomID && connection.state === signalR.HubConnectionState.Connected) {
    const fetchedMessages = await connection.invoke<Message[]>('LoadMessages', roomID);

    // Fetch movie data for movie messages
    const updatedMessages = await Promise.all(
      fetchedMessages.map(async (message) => {
        if (message.isMovie) {
          const movie = await fetchMovieData(message.body);
          return { ...message, movie };
        }
        return message;
      })
    );

    setMessages(updatedMessages);
  }
};

const fetchMovieData = async (movieId: any) => {
  const response = await fetch(`https://api.themoviedb.org/3/movie/${movieId}?api_key=c9154564bc2ba422e5e0dede6af7f89b&language=lt-LT`);
  const data = await response.json();
  return {
    title: data.title,
    posterUrl: `https://image.tmdb.org/t/p/w500${data.poster_path}`,
  };
};

const messagesEndRef = useRef<null | HTMLDivElement>(null);

const scrollToBottom = () => {
  messagesEndRef.current?.scrollIntoView({ behavior: "smooth" })
}

useEffect(() => {
  scrollToBottom()
}, [messages]);

if (!roomID) {
  return <h4 style={{display:'flex', justifyContent:'center',color:'black'}}>Pasirinkite vartotoją</h4>;
}


return (
  <Box 
  >
{messages.map((message, index) => (
     <div
     key={index}
     className={`message ${message.sender.userName === userName ? 'my-message' : 'other-message'}`}
   >
      {message.isMovie ? (
      
        <ChatMovie 
        title={message.movie?.title}
        Url={message.movie?.posterUrl}
        watchingDate={message.dateTime}
        id={message.id}
        sender={message.sender.userName}
        username ={user}
        />
    
    ) : (
      <div className="message-content">{message.body}</div>
    )}
   </div>
      ))}
    <div 
  ref={messagesEndRef}></div>
  </Box>
);
};

export default  Chat;