import { BrowserRouter as Router } from 'react-router-dom';
import AppRouter from './router/AppRouter';
import Header from './components/common/Header';
import Footer from './components/common/Footer';

function App() {
  return (
    <Router>
      <div className="app-container">
        <Header />
        <main className="flex-grow">
          <AppRouter />
        </main>
        <Footer />
      </div>
    </Router>
  );
}

export default App;