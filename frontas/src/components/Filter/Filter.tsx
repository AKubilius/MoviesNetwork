import Box from "@mui/material/Box/Box";
import axios from "axios";
import React, { useEffect, useState } from "react";
import FilterOption from "./FilterOption";

export default function Filter() {
  const API = "https://api.themoviedb.org/3/";
  const type = "genre/movie/list";
  const [genres, setGenres] = useState<any>([]);

  const fetch = async () => {
    const { data } = await axios.get<any>(`${API}${type}`, {
      params: {
        api_key: "c9154564bc2ba422e5e0dede6af7f89b",
        language: "lt-LT",
      },
    });
    setGenres(data);
    console.log(data);
  };
  useEffect(() => {
    fetch();
  }, []);

  return (
    <div>
      <Box>
        <h3 style={{ textAlign: "center" }}>Kategorijos</h3>
        {genres.genres
          ?.sort((a: any, b: any) => a.name.localeCompare(b.name))
          .map((genre: any) => (
            <FilterOption name={genre.name} id={genre.id} key={genre.id} />
          ))}
      </Box>
    </div>
  );
}
