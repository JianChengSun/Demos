import com.google.common.util.concurrent.ThreadFactoryBuilder;

import java.text.SimpleDateFormat;
import java.util.*;
import java.util.concurrent.*;

public class Main {

    private static SimpleDateFormat simpleDateFormat = new SimpleDateFormat("yyyy-MM-dd HH:mm:ss");

    private static ThreadFactory namedThreadFactory = new ThreadFactoryBuilder().setNameFormat("demo-pool-%d").build();

    private static ExecutorService pool = new ThreadPoolExecutor(5,200,0L, TimeUnit.MILLISECONDS,
            new LinkedBlockingQueue<Runnable>(1024),namedThreadFactory,new ThreadPoolExecutor.AbortPolicy());

    private static  CountDownLatch countDownLatch = new CountDownLatch(100);

    public static void main(String[] args) throws InterruptedException {

        final Set<String> dates = Collections.synchronizedSet(new HashSet<String>());
        for(int i = 0; i < 100; i++){
            final Calendar calendar = Calendar.getInstance();

            final int finalI = i;

            pool.execute(new Runnable() {
                public void run() {
                    synchronized (simpleDateFormat) {
                        //时间增加
                        calendar.add(Calendar.DATE, finalI);
                        //通过simpleDateFormat把时间转换成字符串
                        String dateString = simpleDateFormat.format(calendar.getTime());
                        //把字符串放入Set中
                        dates.add(dateString);
                        //countDown
                        countDownLatch.countDown();
                    }
                }
            });
        }
        countDownLatch.await();
        System.out.println(dates.size());

    }
}
