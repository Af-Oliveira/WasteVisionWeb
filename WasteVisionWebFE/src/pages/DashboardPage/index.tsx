import {
  Card,
  CardContent,
  CardDescription,
  CardHeader,
  CardTitle,
} from "@/components/ui/card";
import { LoadingSpinner } from "@/components/ui/loading-spinner";
import { useAuth } from "@/hooks/useAuth";
import {
  Camera,
  Eye,
  Image,
  TrendingUp,
  Users,
  Target,
  BarChart3,
  FileText,
  Clock,
  AlertCircle,
  Info,
  Bug,
  Shield,
} from "lucide-react";
import { useEffect, useState, useCallback } from "react";
import {
  Bar,
  BarChart,
  CartesianGrid,
  Cell,
  Legend,
  Line,
  LineChart,
  Pie,
  PieChart,
  ResponsiveContainer,
  Tooltip,
  XAxis,
  YAxis,
} from "recharts";
import {
  useUserService,
  usePredictionService,
  useObjectPredictionService,
  useAuthService,
  useLogService, // Add this import
} from "@/di/container";
import { PredictionDTO } from "@/data/dto/prediction-dto";
import { ObjectPredictionDTO } from "@/data/dto/objectPrediction-dto";
import { UserDTO } from "@/data/dto/user-dto";
import { Prediction } from "@/core/domain/Prediction";
import { ObjectPrediction } from "@/core/domain/ObjectPrediction";
import { User } from "@/core/domain/User";
import { UserInfo } from "@/core/domain/Auth";
import { Log } from "@/core/domain/Log"; // Add this import

interface StatCardProps {
  title: string;
  value: string | number;
  icon: React.ReactNode;
  description?: string;
  trend?: "up" | "down" | "neutral";
  colorClass?: string;
}

const StatCard = ({
  title,
  value,
  icon,
  description,
  trend,
  colorClass = "text-primary",
}: StatCardProps) => (
  <Card
    className="overflow-hidden transition-all duration-200 hover:shadow-md border-l-4 dark:border-l-[4px] dark:border-l-primary/60"
    style={{ borderLeftColor: "hsl(var(--primary))" }}
  >
    <CardHeader className="flex flex-row items-center justify-between space-y-0 pb-2 bg-gradient-to-r from-primary/5 to-transparent dark:from-primary/10">
      <CardTitle className="text-sm font-medium">{title}</CardTitle>
      <div className="bg-primary/10 p-2 rounded-full dark:bg-primary/20">
        {icon}
      </div>
    </CardHeader>
    <CardContent className="pt-4">
      <div className="text-2xl font-bold">{value}</div>
      {description && (
        <p className="text-xs text-muted-foreground flex items-center gap-1">
          {trend === "up" && <TrendingUp className="h-3 w-3 text-primary" />}
          {description}
        </p>
      )}
    </CardContent>
  </Card>
);

interface DashboardData {
  predictions: Prediction[];
  objectPredictions: ObjectPrediction[];
  users: User[];
  logs: Log[]; // Add logs to dashboard data
}

interface CategoryData {
  category: string;
  count: number;
  avgConfidence: number;
  fill: string;
}

interface MonthlyPredictions {
  month: string;
  predictions: number;
  detections: number;
}

// Helper function to get log type icon
const getLogTypeIcon = (type: string) => {
  const typeMap: { [key: string]: React.ReactNode } = {
    Error: <AlertCircle className="h-4 w-4 text-red-500" />,
    Auth: <Shield className="h-4 w-4 text-blue-500" />,
    Info: <Info className="h-4 w-4 text-green-500" />,
    Debug: <Bug className="h-4 w-4 text-gray-500" />,
    Detection: <Target className="h-4 w-4 text-purple-500" />,
    Prediction: <Camera className="h-4 w-4 text-orange-500" />,
    User: <Users className="h-4 w-4 text-indigo-500" />,
    Category: <BarChart3 className="h-4 w-4 text-pink-500" />,
    RoboflowModel: <Image className="h-4 w-4 text-cyan-500" />,
    Role: <Shield className="h-4 w-4 text-amber-500" />,
    ObjectPrediction: <Target className="h-4 w-4 text-teal-500" />,
    PredictionResult: <BarChart3 className="h-4 w-4 text-lime-500" />,
  };
  return typeMap[type] || <FileText className="h-4 w-4 text-gray-400" />;
};

// Helper function to get log type color class
const getLogTypeColorClass = (type: string) => {
  const colorMap: { [key: string]: string } = {
    Error: "border-l-red-500 bg-red-50 dark:bg-red-950/20",
    Auth: "border-l-blue-500 bg-blue-50 dark:bg-blue-950/20",
    Info: "border-l-green-500 bg-green-50 dark:bg-green-950/20",
    Debug: "border-l-gray-500 bg-gray-50 dark:bg-gray-950/20",
    Detection: "border-l-purple-500 bg-purple-50 dark:bg-purple-950/20",
    Prediction: "border-l-orange-500 bg-orange-50 dark:bg-orange-950/20",
    User: "border-l-indigo-500 bg-indigo-50 dark:bg-indigo-950/20",
    Category: "border-l-pink-500 bg-pink-50 dark:bg-pink-950/20",
    RoboflowModel: "border-l-cyan-500 bg-cyan-50 dark:bg-cyan-950/20",
    Role: "border-l-amber-500 bg-amber-50 dark:bg-amber-950/20",
    ObjectPrediction: "border-l-teal-500 bg-teal-50 dark:bg-teal-950/20",
    PredictionResult: "border-l-lime-500 bg-lime-50 dark:bg-lime-950/20",
  };
  return colorMap[type] || "border-l-gray-400 bg-gray-50 dark:bg-gray-950/20";
};

export function DashboardPage() {
  const objectPredictionApi = useObjectPredictionService();
  const predictionApi = usePredictionService();
  const userApi = useUserService();
  const authApi = useAuthService();
  const logApi = useLogService(); // Add log service

  const { user } = useAuth();

  const [loading, setLoading] = useState(true);
  const [dashboardData, setDashboardData] = useState<DashboardData>({
    predictions: [],
    objectPredictions: [],
    users: [],
    logs: [], // Initialize logs
  });

  const loadDashboardData = useCallback(async () => {
    try {
      setLoading(true);

      // Load data from all APIs in parallel
      const [predictions, objectPredictions, users, logs] = await Promise.all([
        predictionApi.getPredictions(),
        objectPredictionApi.getObjectPredictions(),
        userApi.getUsers(),
        logApi.getLogs(), // Add logs to parallel loading
      ]);

      setDashboardData({
        predictions,
        objectPredictions,
        users,
        logs,
      });
    } catch (err) {
      console.error("Failed to load dashboard data:", err);
    } finally {
      setLoading(false);
    }
  }, [predictionApi, objectPredictionApi, userApi, logApi]); // Add logApi to dependencies

  useEffect(() => {
    loadDashboardData();
  }, [loadDashboardData]);

  // Calculate statistics from real data
  const calculateStatistics = () => {
    const { predictions, objectPredictions, users } = dashboardData;

    const totalPredictions = predictions.length;
    const totalDetections = objectPredictions.length;
    const activeUsers = users.filter((u) => u.active).length;
    const totalUsers = users.length;

    // Calculate average confidence
    const avgConfidence =
      objectPredictions.length > 0
        ? (
            objectPredictions.reduce(
              (sum, obj) => sum + parseFloat(obj.confidence),
              0
            ) / objectPredictions.length
          ).toFixed(1)
        : "0";

    // Calculate recent activity (last 7 days)
    const sevenDaysAgo = new Date();
    sevenDaysAgo.setDate(sevenDaysAgo.getDate() - 7);

    const recentPredictions = predictions.filter(
      (p) => new Date(p.date) >= sevenDaysAgo
    ).length;

    return {
      totalPredictions,
      totalDetections,
      activeUsers,
      totalUsers,
      avgConfidence: `${avgConfidence}%`,
      recentActivity: recentPredictions,
    };
  };

  // Process category data for visualization
  const getCategoryData = (): CategoryData[] => {
    const { objectPredictions } = dashboardData;
    const categoryMap = new Map<
      string,
      { count: number; totalConfidence: number }
    >();

    objectPredictions.forEach((obj) => {
      const category = obj.category;
      const confidence = parseFloat(obj.confidence);

      if (categoryMap.has(category)) {
        const existing = categoryMap.get(category)!;
        categoryMap.set(category, {
          count: existing.count + 1,
          totalConfidence: existing.totalConfidence + confidence,
        });
      } else {
        categoryMap.set(category, {
          count: 1,
          totalConfidence: confidence,
        });
      }
    });

    const colors = [
      "hsl(var(--chart-1))",
      "hsl(var(--chart-2))",
      "hsl(var(--chart-3))",
      "hsl(var(--chart-4))",
      "hsl(var(--chart-5))",
    ];

    return Array.from(categoryMap.entries())
      .map(([category, data], index) => ({
        category,
        count: data.count,
        avgConfidence: parseFloat(
          (data.totalConfidence / data.count).toFixed(1)
        ),
        fill: colors[index % colors.length],
      }))
      .sort((a, b) => b.count - a.count)
      .slice(0, 5); // Top 5 categories
  };

  // Process monthly predictions data
  const getMonthlyData = (): MonthlyPredictions[] => {
    const { predictions, objectPredictions } = dashboardData;
    const monthlyData = new Map<
      string,
      { predictions: number; detections: number }
    >();

    // Get last 6 months
    const months = [];
    for (let i = 6; i >= 0; i--) {
      const date = new Date();
      date.setMonth(date.getMonth() - i);
      const monthKey = date.toLocaleString("default", { month: "short" });
      months.push(monthKey);
      monthlyData.set(monthKey, { predictions: 0, detections: 0 });
    }

    predictions.forEach((prediction) => {
    
      const date = new Date(prediction.date);
      const monthKey = date.toLocaleString("default", { month: "short" });
      if (monthlyData.has(monthKey)) {
        const existing = monthlyData.get(monthKey)!;
        monthlyData.set(monthKey, {
          ...existing,
          predictions: existing.predictions + 1,
        });
      }
    });

    // Count detections by matching prediction IDs
    objectPredictions.forEach((objPred) => {
      const prediction = predictions.find((p) => p.id === objPred.predictionId);
      if (prediction) {
        const date = new Date(prediction.date);
        const monthKey = date.toLocaleString("default", { month: "short" });
        if (monthlyData.has(monthKey)) {
          const existing = monthlyData.get(monthKey)!;
          monthlyData.set(monthKey, {
            ...existing,
            detections: existing.detections + 1,
          });
        }
      }
    });

    return months.map((month) => ({
      month,
      ...monthlyData.get(month)!,
    }));
  };

  // Get confidence distribution
  const getConfidenceDistribution = () => {
    const { objectPredictions } = dashboardData;
    const ranges = [
      {
        name: "90-100%",
        min: 0.9,
        max: 1.0,
        count: 0,
        color: "hsl(var(--chart-1))",
      },
      {
        name: "80-89%",
        min: 0.8,
        max: 0.89,
        count: 0,
        color: "hsl(var(--chart-2))",
      },
      {
        name: "70-79%",
        min: 0.7,
        max: 0.79,
        count: 0,
        color: "hsl(var(--chart-3))",
      },
      {
        name: "60-69%",
        min: 0.6,
        max: 0.69,
        count: 0,
        color: "hsl(var(--chart-4))",
      },
      {
        name: "<60%",
        min: 0,
        max: 0.59,
        count: 0,
        color: "hsl(var(--chart-5))",
      },
    ];

    objectPredictions.forEach((obj) => {
      const confidence = parseFloat(obj.confidence);
      const range = ranges.find((r) => confidence >= r.min && confidence <= r.max);
      if (range) range.count++;
    });

    return ranges.filter((r) => r.count > 0);
  };

  if (loading) {
    return (
      <div className="flex flex-1 items-center justify-center min-h-[600px]">
        <LoadingSpinner />
      </div>
    );
  }

  const statistics = calculateStatistics();
  const categoryData = getCategoryData();
  const monthlyData = getMonthlyData();
  const confidenceDistribution = getConfidenceDistribution();

  return (
    <div className="max-w-[1600px] mx-auto px-4 py-6 w-full">
      <div className="flex flex-col md:flex-row justify-between items-start md:items-center mb-8">
        <h1 className="text-3xl font-bold text-foreground">
          Welcome back, {user?.username || "User"}
        </h1>
        <div className="flex items-center gap-2 px-3 py-2 bg-primary/5 rounded-md text-primary text-sm mt-3 md:mt-0 dark:bg-primary/20">
          <Eye className="h-4 w-4" />
          <span>Detection System: Active</span>
        </div>
      </div>

      <div className="grid gap-5 md:grid-cols-2 lg:grid-cols-4">
        <StatCard
          title="Total Predictions"
          value={statistics.totalPredictions}
          icon={<Camera className="h-4 w-4 text-primary" />}
          description={`${statistics.recentActivity} in last 7 days`}
          trend="up"
        />
        <StatCard
          title="Object Detections"
          value={statistics.totalDetections}
          icon={<Target className="h-4 w-4 text-primary" />}
          description="Objects detected"
        />
        <StatCard
          title="Average Confidence"
          value={statistics.avgConfidence}
          icon={<BarChart3 className="h-4 w-4 text-primary" />}
          description="Detection accuracy"
        />
        <StatCard
          title="Active Users"
          value={`${statistics.activeUsers}/${statistics.totalUsers}`}
          icon={<Users className="h-4 w-4 text-primary" />}
          description="User engagement"
        />
      </div>

      <div className="mt-8 grid gap-5 md:grid-cols-1 lg:grid-cols-2">
        <Card className="shadow-sm hover:shadow-md transition-all duration-300 border border-border">
          <CardHeader className="bg-gradient-to-r from-primary/5 to-transparent dark:from-primary/10">
            <CardTitle>Monthly Activity</CardTitle>
            <CardDescription>
              Predictions and detections over time
            </CardDescription>
          </CardHeader>
          <CardContent className="pt-6">
            <div className="w-full h-[300px]">
              <ResponsiveContainer width="100%" height="100%">
                <LineChart data={monthlyData}>
                  <CartesianGrid
                    strokeDasharray="3 3"
                    opacity={0.2}
                    stroke="hsl(var(--border))"
                  />
                  <XAxis
                    dataKey="month"
                    tick={{ fill: "hsl(var(--foreground))" }}
                  />
                  <YAxis tick={{ fill: "hsl(var(--foreground))" }} />
                  <Tooltip
                    content={({ active, payload, label }) => {
                      if (active && payload && payload.length) {
                        return (
                          <div className="bg-card p-3 border rounded-md shadow-md text-card-foreground">
                            <p className="font-medium">{label}</p>
                            {payload.map((entry, index) => (
                              <div
                                key={index}
                                className="flex items-center gap-2 text-sm"
                              >
                                <div
                                  className="w-3 h-3 rounded-full"
                                  style={{ backgroundColor: entry.color }}
                                ></div>
                                <span>
                                  {entry.name}: {entry.value}
                                </span>
                              </div>
                            ))}
                          </div>
                        );
                      }
                      return null;
                    }}
                  />
                  <Legend
                    formatter={(value) => (
                      <span className="text-foreground">{value}</span>
                    )}
                  />
                  <Line
                    type="monotone"
                    dataKey="predictions"
                    stroke="hsl(var(--chart-1))"
                    strokeWidth={2}
                    dot={{ r: 4, fill: "hsl(var(--chart-1))" }}
                    activeDot={{ r: 6 }}
                    name="Predictions"
                  />
                  <Line
                    type="monotone"
                    dataKey="detections"
                    stroke="hsl(var(--chart-2))"
                    strokeWidth={2}
                    dot={{ r: 4, fill: "hsl(var(--chart-2))" }}
                    activeDot={{ r: 6 }}
                    name="Detections"
                  />
                </LineChart>
              </ResponsiveContainer>
            </div>
          </CardContent>
        </Card>

        <Card className="shadow-sm hover:shadow-md transition-all duration-300 border border-border">
          <CardHeader className="bg-gradient-to-r from-primary/5 to-transparent dark:from-primary/10">
            <CardTitle>Detection Categories</CardTitle>
            <CardDescription>Most detected object categories</CardDescription>
          </CardHeader>
          <CardContent className="pt-6">
            <div className="w-full h-[300px]">
              <ResponsiveContainer width="100%" height="100%">
                <BarChart
                  data={categoryData}
                  layout="vertical"
                  margin={{ left: 80 }}
                >
                  <CartesianGrid
                    strokeDasharray="3 3"
                    horizontal={true}
                    vertical={false}
                    opacity={0.2}
                    stroke="hsl(var(--border))"
                  />
                  <XAxis
                    type="number"
                    tick={{ fill: "hsl(var(--foreground))" }}
                  />
                  <YAxis
                    type="category"
                    dataKey="category"
                    tick={{ fontSize: 14, fill: "hsl(var(--foreground))" }}
                    width={80}
                  />
                  <Tooltip
                    content={({ active, payload }) => {
                      if (active && payload && payload.length) {
                        const data = payload[0].payload;
                        return (
                          <div className="rounded-lg border bg-card p-3 shadow-md text-card-foreground">
                            <p className="font-medium">{data.category}</p>
                            <p className="text-sm">Count: {data.count}</p>
                            <p className="text-sm">
                              Avg Confidence: {data.avgConfidence}%
                            </p>
                          </div>
                        );
                      }
                      return null;
                    }}
                  />
                  <Bar
                    dataKey="count"
                    fill="hsl(var(--primary))"
                    radius={[0, 4, 4, 0]}
                    barSize={25}
                  >
                    {categoryData.map((entry, index) => (
                      <Cell key={`cell-${index}`} fill={entry.fill} />
                    ))}
                  </Bar>
                </BarChart>
              </ResponsiveContainer>
            </div>
          </CardContent>
        </Card>
      </div>

      <div className="mt-5 grid gap-5 md:grid-cols-1 lg:grid-cols-2">
        <Card className="shadow-sm hover:shadow-md transition-all duration-300 border border-border">
          <CardHeader className="bg-gradient-to-r from-primary/5 to-transparent dark:from-primary/10">
            <CardTitle>Recent Predictions</CardTitle>
            <CardDescription>
              Latest image predictions and detections
            </CardDescription>
          </CardHeader>
          <CardContent>
            <div className="space-y-4">
              {dashboardData.predictions.slice(0, 4).map((prediction, index) => {
                const detectionCount = dashboardData.objectPredictions.filter(
                  (obj) => {
                    return obj.predictionId === prediction.id;
                  }
                ).length;

                return (
                  <div
                    key={index}
                    className="flex items-center gap-4 p-3 rounded-lg border border-border bg-card hover:bg-accent/50 transition-colors"
                  >
                    <div className="bg-primary/10 p-2 rounded-full dark:bg-primary/20">
                      <Image className="h-5 w-5 text-primary" />
                    </div>
                    <div className="flex-1">
                      <div className="flex justify-between items-center">
                        <p className="font-medium">
                          Prediction #{prediction.id}
                        </p>
                        <span className="text-sm text-primary font-medium">
                          {detectionCount} objects
                        </span>
                      </div>
                      <div className="flex justify-between items-center mt-1">
                        <span className="text-xs text-muted-foreground">
                          {new Date(prediction.date).toLocaleDateString()}
                        </span>
                        <span className="text-xs">
                          Model: {prediction.modelId}
                        </span>
                      </div>
                    </div>
                  </div>
                );
              })}
            </div>
          </CardContent>
        </Card>

        <Card className="shadow-sm hover:shadow-md transition-all duration-300 border border-border">
          <CardHeader className="bg-gradient-to-r from-primary/5 to-transparent dark:from-primary/10">
            <CardTitle>Confidence Distribution</CardTitle>
            <CardDescription>Detection confidence levels</CardDescription>
          </CardHeader>
          <CardContent>
            <div className="w-full h-[250px] flex justify-center">
              <ResponsiveContainer width="100%" height="100%">
                <PieChart>
                  <Pie
                    data={confidenceDistribution}
                    cx="50%"
                    cy="50%"
                    innerRadius={60}
                    outerRadius={90}
                    paddingAngle={4}
                    dataKey="count"
                    nameKey="name"
                    label={({
                      cx,
                      cy,
                      midAngle,
                      innerRadius,
                      outerRadius,
                      percent,
                    }) => {
                      const RADIAN = Math.PI / 180;
                      const radius = outerRadius * 1.1;
                      const x = cx + radius * Math.cos(-midAngle * RADIAN);
                      const y = cy + radius * Math.sin(-midAngle * RADIAN);

                      return (
                        <text
                          x={x}
                          y={y}
                          fill="hsl(var(--foreground))"
                          textAnchor={x > cx ? "start" : "end"}
                          dominantBaseline="central"
                          className="text-xs"
                        >
                          {`${(percent * 100).toFixed(0)}%`}
                        </text>
                      );
                    }}
                  >
                    {confidenceDistribution.map((entry, index) => (
                      <Cell key={`cell-${index}`} fill={entry.color} />
                    ))}
                  </Pie>
                  <Legend />
                  <Tooltip
                    content={({ active, payload }) => {
                      if (active && payload && payload.length) {
                        const data = payload[0].payload;
                        return (
                          <div className="rounded-lg border bg-card p-3 shadow-md text-card-foreground">
                            <p className="font-medium">{data.name}</p>
                            <p className="text-sm">Count: {data.count}</p>
                          </div>
                        );
                      }
                      return null;
                    }}
                  />
                </PieChart>
              </ResponsiveContainer>
            </div>
            <div className="flex justify-center gap-4 mt-4 flex-wrap">
              {confidenceDistribution.map((item, index) => (
                <div key={index} className="flex items-center gap-2">
                  <div
                    className="w-3 h-3 rounded-full"
                    style={{ backgroundColor: item.color }}
                  ></div>
                  <span className="text-xs text-foreground">{item.name}</span>
                </div>
              ))}
            </div>
          </CardContent>
        </Card>
      </div>

      {/* NEW LOGS SECTION */}
      <div className="mt-8">
        <Card className="shadow-sm hover:shadow-md transition-all duration-300 border border-border">
          <CardHeader className="bg-gradient-to-r from-primary/5 to-transparent dark:from-primary/10">
            <div className="flex items-center justify-between">
              <div>
                <CardTitle className="flex items-center gap-2">
                  <FileText className="h-5 w-5 text-primary" />
                  System Logs
                </CardTitle>
                <CardDescription>
                  Recent system activity and events
                </CardDescription>
              </div>
              <div className="flex items-center gap-2 text-sm text-muted-foreground">
                <Clock className="h-4 w-4" />
                <span>Last {Math.min(dashboardData.logs.length, 10)} entries</span>
              </div>
            </div>
          </CardHeader>
          <CardContent className="p-0">
            <div className="max-h-[400px] overflow-y-auto">
              {dashboardData.logs.length === 0 ? (
                <div className="flex items-center justify-center py-8 text-muted-foreground">
                  <div className="text-center">
                    <FileText className="h-8 w-8 mx-auto mb-2 opacity-50" />
                    <p>No logs available</p>
                  </div>
                </div>
              ) : (
                <div className="space-y-0">
                  {dashboardData.logs
                    .slice(0, 10)
                    .map((log, index) => (
                      <div
                        key={index}
                        className={`flex items-start gap-4 p-4 border-l-4 hover:bg-accent/30 transition-colors ${getLogTypeColorClass(
                          log.type
                        )} ${
                          index !== dashboardData.logs.slice(0, 10).length - 1
                            ? "border-b border-border/50"
                            : ""
                        }`}
                      >
                        <div className="flex-shrink-0 mt-0.5">
                          {getLogTypeIcon(log.type)}
                        </div>
                        <div className="flex-1 min-w-0">
                          <div className="flex items-center justify-between gap-2 mb-1">
                            <div className="flex items-center gap-2">
                              <span className="inline-flex items-center px-2 py-1 rounded-full text-xs font-medium bg-primary/10 text-primary">
                                {log.type}
                              </span>
                              <time className="text-xs text-muted-foreground">
                                {new Date(log.timestamp).toLocaleString()}
                              </time>
                            </div>
                          </div>
                          <p className="text-sm text-foreground leading-relaxed">
                            {log.description}
                          </p>
                        </div>
                      </div>
                    ))}
                </div>
              )}
            </div>
            {dashboardData.logs.length > 10 && (
              <div className="border-t border-border bg-muted/30 px-4 py-3">
                <p className="text-xs text-muted-foreground text-center">
                  Showing 10 of {dashboardData.logs.length} total log entries
                </p>
              </div>
            )}
          </CardContent>
        </Card>
      </div>
    </div>
  );
}
