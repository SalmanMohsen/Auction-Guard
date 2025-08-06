import { useState } from 'react';
import ManageUsers from './ManageUsers';
import ManageProperties from './ManageProperties';
import ManageAuctions from './ManageAuctions';

type Tab = 'users' | 'properties' | 'auctions';

const AdminDashboard = () => {
  const [activeTab, setActiveTab] = useState<Tab>('users');

  const renderContent = () => {
    switch (activeTab) {
      case 'users':
        return <ManageUsers />;
      case 'properties':
        return <ManageProperties />;
      case 'auctions':
        return <ManageAuctions />;
      default:
        return <ManageUsers />;
    }
  };

  return (
    <div className="w-full">
      <div className="mb-4 border-b border-gray-200">
        <ul className="flex flex-wrap -mb-px text-sm font-medium text-center">
          <li className="mr-2">
            <button onClick={() => setActiveTab('users')} className={`inline-block p-4 rounded-t-lg ${activeTab === 'users' ? 'border-b-2 border-blue-500' : ''}`}>
              Manage Users
            </button>
          </li>
          <li className="mr-2">
            <button onClick={() => setActiveTab('properties')} className={`inline-block p-4 rounded-t-lg ${activeTab === 'properties' ? 'border-b-2 border-blue-500' : ''}`}>
              Manage Properties
            </button>
          </li>
          <li>
            <button onClick={() => setActiveTab('auctions')} className={`inline-block p-4 rounded-t-lg ${activeTab === 'auctions' ? 'border-b-2 border-blue-500' : ''}`}>
              Manage Auctions
            </button>
          </li>
        </ul>
      </div>
      <div>
        {renderContent()}
      </div>
    </div>
  );
};

export default AdminDashboard;
