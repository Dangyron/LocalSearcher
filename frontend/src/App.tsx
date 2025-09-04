import { BrowserRouter as Router, Routes, Route } from 'react-router-dom';
import { useEffect } from 'react';
import Layout from './components/Layout';
import SearchPage from './pages/SearchPage';
import { useThemeStore } from './store/themeStore';
import './styles/main.scss';

export default function App() {
  const { theme } = useThemeStore();

  useEffect(() => {
    document.documentElement.setAttribute('data-theme', theme);
  }, [theme]);

  return (
    <Router>
      <Layout>
        <Routes>
          <Route path="/" element={<SearchPage />} />
        </Routes>
      </Layout>
    </Router>
  );
}
