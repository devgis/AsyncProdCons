using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;

namespace AsyncProdCons
{
    public partial class MainForm : Form
    {
        bool started = true;
        List<Thread> threads = new List<Thread>();
        int pcount = 3;
        int ccount = 5;
        Random rd = new Random();

        public MainForm()
        {
            InitializeComponent();

            for (int i = 0; i < pcount; i++)
            {
                Thread producerThread = new Thread(ProducerDoWork);
                producerThread.Start();
                producerThread.Name = "生产者" + i;
                threads.Add(producerThread);
            }

            for (int i = 0; i < ccount; i++)
            {
                Thread consumerThread = new Thread(ConsumerDoWork);
                consumerThread.Start();
                consumerThread.Name = "消费者" + i;
                threads.Add(consumerThread);
            }
        }

        private void btStart_Click(object sender, EventArgs e)
        {
            started = true;
        }

        private void btEnd_Click(object sender, EventArgs e)
        {
            started = false;
        }

        //输出日志
        private void ShowLog(TextBox txtLog,string log,bool clear=false)
        {
            if (this.IsHandleCreated)
            {
                this.Invoke((EventHandler)(delegate
                {
                    if (txtLog.Text.Length >= 65535 || clear)
                    {
                        txtLog.Clear();
                    }
                    txtLog.Text += log + "\r\n";

                    //滚动到底部
                    txtLog.Select(txtLog.Text.Length, 0);
                    txtLog.ScrollToCaret();

                }));
            }
        }
        #region 生产者消费者相关代码
        class Item
        {
            public string name = "一个产品";
        }

        // 产品队列缓存
        Queue<Item> queue = new Queue<Item>();
        const int BUFFER_SIZE = 5;
        Semaphore fillCount = new Semaphore(0, BUFFER_SIZE);
        Semaphore emptyCount = new Semaphore(BUFFER_SIZE, BUFFER_SIZE);
        // 将产品放入缓存中
        void putItemIntoBuffer(Item item)
        {
            queue.Enqueue(item);
            //Console.WriteLine("将" + item.name + "放入队列，现在有" + queue.Count + "个");
            ShowLog(tbTotal, "Total:" + queue.Count,true);
        }

        // 从缓存中获取产品
        Item removeItemFromBuffer()
        {
            //var item = queue.Peek();
            var item = queue.Dequeue();
            //Console.WriteLine("将" + item.name + "取出队列，现在有" + queue.Count + "个");
            ShowLog(tbTotal, "Total:" + queue.Count,true);
            return item;
        }

        // 生产产品
        Item ProduceItem(int number)
        {
            //Thread.Sleep(TimeSpan.FromSeconds(1));
            Item item = new Item() { name = "产品" + number };
            ShowLog(tbP, "[" + Thread.CurrentThread.Name + "]生产了" + item.name);
            return item;
        }

        // 消费产品
        void ConsumItem(Item item)
        {
            //Thread.Sleep(TimeSpan.FromSeconds(4));
            ShowLog(tbC, "[" + Thread.CurrentThread.Name + "]消费了" + item.name);
        }

        //生产者工作线程
        void ProducerDoWork()
        {
            int productNumber = 1;
            while (started)
            {
                var item = ProduceItem(productNumber++);

                // 还有生产权限时，进入下面的代码
                emptyCount.WaitOne();
                // 将产品放入buffer中
                putItemIntoBuffer(item);
                // 释放一个拿去权限
                fillCount.Release();
                Thread.Sleep(rd.Next(100,1000)); //隨機等待
            }
        }
        //消费者者工作线程
        void ConsumerDoWork()
        {
            while (started)
            {
                // 等待一个拿去权限
                fillCount.WaitOne();
                // 移除一个物品
                var item = removeItemFromBuffer();
                // 释放一个生产权限
                emptyCount.Release();
                ConsumItem(item);
                Thread.Sleep(rd.Next(500, 3000)); //隨機等待
            }
        }

        #endregion
        //窗体关闭时先关闭线程
        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            for (int i = 0; i < threads.Count; i++)
            {
                threads[i].Abort();
                threads[i].Join();
            }
            Application.Exit();
        }

    }
}
