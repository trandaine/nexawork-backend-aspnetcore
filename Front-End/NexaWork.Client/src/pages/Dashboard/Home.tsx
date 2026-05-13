import React from 'react';
import PageMeta from "../../components/common/PageMeta";

// === COMMENT LẠI CÁC IMPORT CŨ CỦA TEMPLATE BẰNG DẤU // ===
// import EcommerceMetrics from "../../components/ecommerce/EcommerceMetrics";
// import MonthlySalesChart from "../../components/ecommerce/MonthlySalesChart";
// import StatisticsChart from "../../components/ecommerce/StatisticsChart";
// import MonthlyTarget from "../../components/ecommerce/MonthlyTarget";
// import RecentOrders from "../../components/ecommerce/RecentOrders";
// import DemographicCard from "../../components/ecommerce/DemographicCard";


import { CreatePostForm } from '../../features/post/components/CreatePost';
// import { PostFeed } from '../../features/post/components/PostFeed';

export default function Home() {
  return (
    <>
      <PageMeta
        title="Home | NexaWork Platform"
        description="Connect with students and recruiters on NexaWork"
      />
      
      {/* === GIAO DIỆN MỚI CỦA NEXAWORK === */}
      {/* Sử dụng max-w-2xl và mx-auto để thu hẹp nội dung và căn giữa màn hình */}
      <div className="max-w-2xl mx-auto py-8 px-4">
        {/* Component tạo bài viết */}
        <CreatePostForm />

        {/* Component hiển thị danh sách bài viết */}
        <div className="mt-6">
          {/* <PostFeed /> */}
        </div>
      </div>

      {/* === COMMENT LẠI GIAO DIỆN CŨ CỦA TEMPLATE BẰNG {/* ... *} === */}
      {/* <div className="grid grid-cols-12 gap-4 md:gap-6 mt-10">
        <div className="col-span-12 space-y-6 xl:col-span-7">
          <EcommerceMetrics />
          <MonthlySalesChart />
        </div>

        <div className="col-span-12 xl:col-span-5">
          <MonthlyTarget />
        </div>

        <div className="col-span-12">
          <StatisticsChart />
        </div>

        <div className="col-span-12 xl:col-span-5">
          <DemographicCard />
        </div>

        <div className="col-span-12 xl:col-span-7">
          <RecentOrders />
        </div>
      </div>
      */}
    </>
  );
}