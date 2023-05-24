import React from "react";
import { useEffect, useState } from "react";
import axios from "axios";
import Friend from "./Friend";

const AddFriends = () => {
  const [friends, setFriends] = useState<any>([]);
  const token = `Bearer ${sessionStorage.getItem("token")}`;
  const fetch = async () => {
    const { data } = await axios.get(`https://localhost:7019/user/friends`, {
      headers: {
        "Content-Type": "application/json",
        Accept: "application/json",
        Authorization: token,
      },
    });
    setFriends(data);
    console.log(data);
  };
  useEffect(() => {
    fetch();
  }, []);

  return (
    <div>
      <h3 style={{ textAlign: "center" }}>Pasirinkite draugą</h3>
      {friends.map((user: any, index: number) => (
        <>
          <Friend
            username={user.userName}
            name={user.name}
            surname={user.surname}
            image64={user.profileImageBase64}
            id={user.id}
            index={index}
          />
        </>
      ))}
    </div>
  );
};

export default AddFriends;
