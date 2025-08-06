import Register from '../features/auth/Register';

const RegisterPage = () => {
  return (
    <div className="container mx-auto px-4 flex justify-center items-center h-screen">
      <div className="w-full max-w-md">
        <h1 className="text-3xl font-bold text-center mb-6">Create an Account</h1>
        <Register />
      </div>
    </div>
  );
};

export default RegisterPage;