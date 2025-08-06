import Login from '../features/auth/Login';

const LoginPage = () => {
  return (
    <div className="container mx-auto px-4 flex justify-center items-center h-screen">
      <div className="w-full max-w-md">
        <h1 className="text-3xl font-bold text-center mb-6">Login</h1>
        <Login />
      </div>
    </div>
  );
};

export default LoginPage;