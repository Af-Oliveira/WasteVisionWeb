import React from "react";
import { Link, useNavigate } from "react-router-dom";
import { buttonVariants } from "@/components/ui/button";
import { useAuth } from "@/hooks/useAuth";
import { Leaf, Recycle, Camera, BarChart, Target } from "lucide-react";

export const LandingPage = () => {
  const { user } = useAuth();
  const navigate = useNavigate();

  const handleExploreClick = () => {
    if (user) {
      navigate("/scan");
    } else {
      navigate("/login");
    }
  };

  const features = [
    {
      icon: <Camera className="h-10 w-10 text-primary" />, // Adjusted size for new container
      title: "Advanced Computer Vision",
      description:
        "Cutting-edge AI technology that accurately identifies recyclable materials with high precision.",
    },
    {
      icon: <Recycle className="h-10 w-10 text-primary" />,
      title: "Smart Waste Sorting",
      description:
        "Automated sorting solutions that streamline recycling processes and reduce human error.",
    },
    {
      icon: <BarChart className="h-10 w-10 text-primary" />,
      title: "Comprehensive Analytics",
      description:
        "Detailed insights into waste composition, recycling rates, and environmental impact.",
    },
  ];

  return (
    <div className="min-h-screen bg-background text-foreground">
      {/* Hero Section */}
      <div className="relative overflow-hidden bg-gradient-to-br from-background via-muted/30 to-background">
        <div className="mx-auto max-w-7xl px-4 py-28 sm:px-6 md:py-26 lg:px-8">
          <div className="text-center">
            <div className="mb-8 flex justify-center">
              <Leaf className="h-20 w-20 text-primary" />
            </div>
            <h1 className="text-4xl font-extrabold tracking-tight text-foreground sm:text-5xl md:text-6xl lg:text-7xl">
              <span className="block">Welcome to</span>
              <span className="mt-2 block text-primary">WasteVision AI</span>
            </h1>
            <p className="mx-auto mt-6 max-w-xl text-lg text-muted-foreground sm:text-xl md:mt-8 md:text-2xl">
              Revolutionizing waste management through intelligent computer
              vision. Transforming recycling with AI-powered precision and
              sustainability.
            </p>
            <div className="mt-10 md:mt-12">
              <button
                onClick={handleExploreClick}
                className={buttonVariants({
                  variant: "default",
                  size: "lg",
                  className:
                    "px-10 py-4 text-xl font-semibold shadow-lg transition-all duration-300 hover:scale-105 hover:shadow-primary/40",
                })}
              >
                {user ? "Start Detecting!" : "Login to Explore!"}
              </button>
            </div>
          </div>
        </div>
      </div>

      {/* Features Section */}
      <div className="bg-muted/90 py-20 md:py-26">
        <div className="mx-auto max-w-7xl px-4 sm:px-6 lg:px-8">
          <div className="mb-16 text-center md:mb-20">
            <h2 className="mb-6 text-3xl font-bold tracking-tight text-foreground sm:text-4xl md:text-5xl">
              Our Technology
            </h2>
            <p className="mx-auto max-w-3xl text-lg text-muted-foreground sm:text-xl">
              WasteVision AI leverages state-of-the-art machine learning to
              revolutionize waste management and promote sustainable practices.
            </p>
          </div>
          <div className="grid gap-8 md:grid-cols-3 lg:gap-12">
            {features.map((feature, index) => (
              <div
                key={index}
                className="flex transform flex-col items-center rounded-xl bg-card p-8 text-card-foreground shadow-xl ring-1 ring-border/30 transition-all duration-300 hover:-translate-y-1 hover:shadow-2xl"
              >
                <div className="mb-6 inline-flex flex-shrink-0 rounded-full bg-primary/10 p-4">
                  {feature.icon}
                </div>
                <h3 className="mb-4 text-center text-2xl font-semibold">
                  {feature.title}
                </h3>
                <p className="text-center text-base leading-relaxed text-muted-foreground">
                  {feature.description}
                </p>
              </div>
            ))}
          </div>
        </div>
      </div>

      {/* Call to Action Section */}
      <div className="bg-primary py-20 text-primary-foreground md:py-28">
        <div className="mx-auto max-w-4xl px-4 text-center sm:px-6 lg:px-8">
          <div className="mb-8 flex justify-center">
            <Target className="h-16 w-16" /> {/* Inherits text-primary-foreground */}
          </div>
          <h2 className="mb-6 text-3xl font-bold tracking-tight sm:text-4xl md:text-5xl">
            Join the Sustainable Revolution
          </h2>
          <p className="mx-auto mb-10 max-w-2xl text-lg text-primary-foreground/80 sm:text-xl">
            Be part of the solution. WasteVision AI is committed to
            transforming waste management through innovative technology and
            environmental consciousness.
          </p>
        </div>
      </div>
    </div>
  );
};

export default LandingPage;
