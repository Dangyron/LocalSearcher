import React from 'react';
import { Link } from 'react-router-dom';
import ThemeToggle from './ThemeToggle';

interface LayoutProps {
    children: React.ReactNode;
}

export default function Layout({ children }: LayoutProps) {
    return (
        <div className="app-container">
            <header className="app-header">
                <div className="header-content">
                    <div className="logo-area">
                        <Link to="/" className="logo-link">
                            <div className="logo">
                                <span>LS</span>
                            </div>
                            <h1 className="app-title">Local search</h1>
                        </Link>
                    </div>
                    <nav className="app-nav">
                        <ul className="nav-list">
                            <li className="nav-item">
                                <Link to="/" className="nav-link">
                                    Search
                                </Link>
                            </li>
                            <li className="nav-item">
                                <ThemeToggle />
                            </li>
                        </ul>
                    </nav>
                </div>
            </header>

            <main className="app-main">
                {children}
            </main>

            <footer className="app-footer">
                <div className="footer-content">
                    <p>
                        © {new Date().getFullYear()} LocalSearcher - Search your local documents
                    </p>
                </div>
            </footer>
        </div>
    );
}