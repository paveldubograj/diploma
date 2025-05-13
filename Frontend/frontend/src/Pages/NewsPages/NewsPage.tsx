// components/NewsPage.tsx
import ErrorBoundary from '../../components/ErrorBoundary';
import NewsList from '../../components/NewsComponents/NewsList';

const NewsPage: React.FC = () => {
  return (
    <div className="flex-grow-1">
      <h2>News</h2>
      <ErrorBoundary>
      <NewsList/>
      </ErrorBoundary>
    </div>
  );
};

export default NewsPage;
