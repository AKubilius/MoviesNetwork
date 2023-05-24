// src/components/SearchBar.tsx
import React, { useState, useEffect } from 'react';
import { TextField, IconButton } from '@mui/material';
import { Search } from '@mui/icons-material';
import axios from 'axios';

interface SearchBarProps {
  onResults: (results: any[]) => void;
}

const SearchBar: React.FC<SearchBarProps> = ({ onResults }) => {
  const [query, setQuery] = useState('');
  const [searchTerm, setSearchTerm] = useState('');

  useEffect(() => {
    const debounceTimeout = setTimeout(() => {
      searchMovies();
    }, 500); // Debounce time in milliseconds

    return () => {
      clearTimeout(debounceTimeout);
    };
  }, [searchTerm]);

  const searchMovies = async () => {
    if (searchTerm === '') {
      onResults([]);
      return;
    }

    try {
      const response = await axios.get(
        `https://api.themoviedb.org/3/search/movie?api_key=c9154564bc2ba422e5e0dede6af7f89b&language=lt-LT&query=${searchTerm}`
      );
      onResults(response.data.results);
    } catch (error) {
      console.error('Error fetching movie data:', error);
    }
  };

  const handleChange = (event: React.ChangeEvent<HTMLInputElement>) => {
    setQuery(event.target.value);
    setSearchTerm(event.target.value);
  };

  return (
    <div style={{margin:10}}>
      <TextField

        label="Ieškoti filmų"
        value={query}
        onChange={handleChange}
        variant="outlined"
        sx={{width: '100%'}}
      />
    </div>
  );
};

export default SearchBar;