// components/NewsPage.tsx
import NewsList from '../../components/NewsComponents/NewsList';

const NewsPage: React.FC = () => {
  return (
    <div className="flex-grow-1">
      <h2>News</h2>
      <NewsList></NewsList>
    </div>
  );
};

export default NewsPage;
